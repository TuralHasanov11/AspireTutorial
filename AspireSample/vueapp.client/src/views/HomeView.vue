<script setup lang="ts">
import { onMounted } from 'vue'
import TheWelcome from '../components/TheWelcome.vue'

onMounted(async () => {
  const response = await fetch('/api/catalog')
  const text = await response.text()
  console.log(text)
})

</script>

<template>
  <main>
    <TheWelcome />
  </main>
</template>
