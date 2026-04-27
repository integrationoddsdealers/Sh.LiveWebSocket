<script setup>
import * as signalR from '@microsoft/signalr/dist/browser/signalr.js';
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { RouterLink } from 'vue-router';

const messages = ref([]);
const connected = ref(false);
const selectedLanguage = ref('en');

let connection = null;

onMounted(() => {
  console.log('App mounted');
});

onBeforeUnmount(async () => {
  await disconnect();
});

async function connect() {
  console.log('Connecting...');

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5014/all-matches-hub?lang=${selectedLanguage.value}&siteId=1`)
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

  connection.on('all-matches-update', (message) => {
    console.log(message);

    const existingMessage = messages.value.findIndex((m) => m.matchId === message.matchId);

    if (existingMessage !== -1) {
      message.updatedAt = new Date().toLocaleTimeString();
      messages.value.splice(existingMessage, 1, message);
    } else {
      messages.value.push(message);
    }
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

async function changeLanguage() {
  if (!connected.value) {
    return;
  }

  try {
    await connection.invoke('ChangeLanguage', 1, selectedLanguage.value);
  } catch (error) {
    console.log('Error changing language', error);
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

  <br />
  <br />

  <button v-if="!connected" @click="connect">Connect</button>
  <button v-if="connected" @click="disconnect">Disconnect</button>
  <br />
  <br />

  <div
    v-for="match in messages"
    :key="match.matchId"
    class="match"
    :style="{ backgroundColor: match.updatedAt ? 'green' : 'red' }"
  >
    <RouterLink
      class="match-id"
      :to="`/match/${match.matchId}?lang=${selectedLanguage}`"
    >
      Match ID: {{ match.matchId }}
    </RouterLink>
    <span v-if="match.updatedAt" class="match-updated-at">Updated At: {{ match.updatedAt }}</span>
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
  color: white;
  text-decoration: underline;
  font-weight: 600;
}

.match .match-updated-at {
  position: absolute;
  top: 2px;
  right: 2px;
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

.message {
  background-color: blue;
  position: relative;
  border: 1px solid #ccc;
  padding: 10px;
  margin: 10px;
}
</style>
