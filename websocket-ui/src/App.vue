<script setup>
import * as signalR from '@microsoft/signalr/dist/browser/signalr.js';
import { onMounted, ref, computed } from 'vue';

const messages = ref([]);
const connected = ref(false);
const selectedLanguage = ref('en');

var connection = null;

onMounted(() => {
  console.log("App mounted");
});

async function connect() {
  console.log("Connecting...");

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5014/match-hub?lang=${selectedLanguage.value}&siteId=1`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

  try {
    await connection.start();
    connected.value = true;
  } catch (error) {
    console.log("Error connecting to hub", error);
  }

  connection.on('notifications', (message) => {
    console.log(message);
  });

  connection.on('match-update', (message) => {
    console.log(message);

    const existingMessage = messages.value.findIndex(m => m.matchId === message.matchId);

    if (existingMessage !== -1) {
      message.updatedAt = new Date().toLocaleTimeString();
      messages.value.splice(existingMessage, 1, message);
    } else {
      messages.value.push(message);
    }
  });


  connection.onclose(() => {
    console.log("Connection closed");
    connected.value = false;
  });
}

async function disconnect() {
  console.log("Disconnecting...");
  await connection.stop();
  connected.value = false;
}

async function changeLanguage() {
  if (!connected.value) {
    return;
  }

  try {
    await connection.invoke('ChangeLanguage', 1, selectedLanguage.value);
  } catch (error) {
    console.log("Error changing language", error);
  }
  messages.value = [];
}
</script>

<template>
  <h1>All matches</h1>

  <select v-model="selectedLanguage" @change="changeLanguage">
    <option value="en">English</option>
    <option value="fr">French</option>
    <option value="es">Spanish</option>
    <option value="de">German</option>
    <option value="it">Italian</option>
    <option value="pt">Portuguese</option>
    <option value="ru">Russian</option>
    <option value="zh">Chinese</option>
  </select>

  <br>
  <br>

  <button @click="connect" v-if="!connected">Connect</button>
  <button @click="disconnect" v-if="connected">Disconnect</button>
  <br>
  <br>

  <!-- <p>{{ matchMessages }}</p> -->
  <div class="match" v-for="match in messages" :key="match.matchId"
      :style="{ backgroundColor: match.updatedAt ? 'green' : 'red' }">

    <span class="match-id">Match ID: {{ match.matchId }}</span> 
    <span v-if="match.updatedAt" class="match-updated-at">Updated At: {{ match.updatedAt }}</span>

    <div class="message" v-for="market in match.markets" :key="market.marketId">

      <p>Market ID: {{ market.marketId }}</p>
      <p>Market Name: {{ market.name }}</p>
      <p v-if="market.updatedAt">Updated At: {{ market.updatedAt }}</p>

      <p>
        Odds:
        <br>
        <span v-for="odd in match.odds.filter(m => m.marketId === market.marketId)" :key="odd.oddId">
          {{ odd.oddId }}
          <br>
        </span>
      </p>


      <span class="odds-badge">{{match.odds.filter(m => m.marketId === market.marketId).length}}</span>
    </div>

  </div>
</template>

<style scoped>
.match {
  position: relative;
  padding-top: 20px;
  display: flex;
  flex-direction: row;
  gap: 10px;
  flex-wrap: wrap;
  margin-bottom: 10px;
  padding-left: 5px;
  padding-right: 5px;
}

.match .match-id {
  position: absolute;
  top: 2px;
  left: 2px;
}

.match .match-updated-at {
  position: absolute;
  top: 2px;
  right: 2px;
}

.odds-badge {
  position: absolute;
  top: 0px;
  right: 0;
  margin: 3px;
  padding: 3px;
  background-color: yellow;
  border-radius: 5px;
  font-size: 12px;
  font-weight: bold;
}

.message {
  background-color: blue;
  position: relative;
  border: 1px solid #ccc;
  padding: 10px;
  margin: 10px;
}
</style>
