import { createRouter, createWebHistory } from 'vue-router'
import Match from '../Match.vue'
import MatchesList from '../views/MatchesList.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'matches-list',
      component: MatchesList,
    },
    {
      path: '/match/:matchId',
      name: 'match',
      component: Match,
      props: true,
    },
  ],
})

export default router
