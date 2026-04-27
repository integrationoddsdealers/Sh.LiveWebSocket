<script setup>
import * as signalR from '@microsoft/signalr/dist/browser/signalr.js';
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { RouterLink, useRoute } from 'vue-router';

const route = useRoute();
const matchId = route.params.matchId;
const selectedLanguage = ref(route.query.lang || 'en');

const messages = ref([]);
const connected = ref(false);
const match = ref({
    matchId: matchId,
    markets: [],
    odds: []
});

let connection = null;

onMounted(async () => {
    console.log(`Match ${matchId} mounted`);
    await connect();
});

onBeforeUnmount(async () => {
    await disconnect();
});

async function connect() {
    console.log('Connecting...');

    connection = new signalR.HubConnectionBuilder()
        .withUrl(`http://localhost:5014/match-hub?lang=${selectedLanguage.value}&siteId=1&matchId=${matchId}`)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    try {
        await connection.start();
        connected.value = true;
    } catch (error) {
        console.log('Error connecting to hub', error);
    }

    connection.on('notifications', (message) => {
        console.log(message);
    });

    connection.on('match-update', (message) => {
        console.log(message);

        try {
            messages.value.push(message);

            message.markets.forEach((market) => {
                const idx = match.value.markets.findIndex(m => m.marketId === market.marketId);
                if (idx === -1) {
                    match.value.markets.push(market);
                } else {
                    match.value.markets[idx] = { ...match.value.markets[idx], ...market };
                }
            });

            message.odds.forEach((odd) => {
                const idx = match.value.odds.findIndex(o => o.oddId === odd.oddId && o.marketId === odd.marketId);
                if (idx === -1) {

                    const marketIdx = match.value.markets.findIndex(m => m.marketId === odd.marketId);
                    if (marketIdx === -1) {
                        match.value.markets.push({
                            marketId: odd.marketId
                        });
                    }
                    
                    match.value.odds.push({
                        oddId: odd.oddId,
                        marketId: odd.marketId,
                        oddValue: odd.oddValue,
                        oddStatus: odd.oddStatus,
                        color: getRandomColor()
                    })
                } else {
                    match.value.odds[idx] = { ...match.value.odds[idx], ...odd, color: getRandomColor() };
                }
            });

            message.oddsUpdate.forEach((odd) => {
                const idx = match.value.odds.findIndex(o => o.oddId === odd.oddId && o.marketId === odd.marketId);

                if (idx === -1) {

                    const marketIdx = match.value.markets.findIndex(m => m.marketId === odd.marketId);
                    if (marketIdx === -1) {
                        match.value.markets.push({
                            marketId: odd.marketId
                        });
                    }

                    match.value.odds.push({
                        oddId: odd.oddId,
                        marketId: odd.marketId,
                        oddValue: odd.oddValue,
                        oddStatus: odd.oddStatus,
                        color: getRandomColor()
                    });
                } else {
                    match.value.odds[idx] = { ...match.value.odds[idx], ...odd, color: getRandomColor() };
                }
            });

            match.value.updatedAt = new Date().toLocaleTimeString();

        } catch (error) {
            console.log('Error updating match', error);
        }
    });

    connection.on('disable-odds', (message) => {
        console.log("Disable odds", message);

        if(!message){
            console.log("Warning: No message received");
            return;
        }

        match.value.odds.forEach((odd) => {
            odd.oddStatus = "disabled";
        });
    });

    connection.onclose(() => {
        console.log('Connection closed');
        connected.value = false;
    });
}

async function disconnect() {
    console.log('Disconnecting...');
    await connection.stop();
    connected.value = false;
}

function getRandomColor() {
    // Generate a random hex color code, e.g., #a3f5c1
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

</script>

<template>
    <div class="match-page">
        <h1>Match Details</h1>
        <p>Match ID: {{ matchId }}</p>

        <RouterLink to="/">Back to all matches</RouterLink>

        <div v-if="match">
            <div v-for="market in match?.markets" :key="market.marketId" class="message">
                <p>Market ID: {{ market.marketId }}</p>
                <p>Market Name: {{ market.name }}</p>
                <p v-if="market.updatedAt">Updated At: {{ market.updatedAt }}</p>

                <div>
                    Odds:
                    <br />
                    <p v-for="odd in match.odds.filter((m) => m.marketId === market.marketId)" :key="odd.oddId" :style="{ backgroundColor: odd.oddStatus === 'disabled' ? 'red' : odd.color }">
                        {{ odd.oddId }} - {{ odd.oddValue }} - {{ odd.oddStatus }}
                        <br />
                    </p>
                </div>

                <span class="odds-badge">{{match?.odds.filter((m) => m.marketId === market.marketId).length}}</span>
                <span v-if="match.updatedAt" class="match-updated-at">Updated At: {{ match.updatedAt }}</span>
            </div>
        </div>

    </div>
</template>

<style scoped>
.match-page {
    padding: 16px;
}

.message {
    background-color: blue;
    position: relative;
    border: 1px solid #ccc;
    padding: 10px;
    margin: 10px;
}

.odds-badge {
  position: absolute;
  top: 0;
  right: 0;
  margin: 3px;
  padding: 3px;
  background-color: yellow;
  border-radius: 5px;
  font-size: 12px;
  font-weight: bold;
}

.match-updated-at {
  position: absolute;
  top: 2px;
  right: 40px;
}
</style>