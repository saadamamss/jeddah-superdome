<template>
  <vue-tel-input v-model="phone" :preferred-countries="['sa', 'us', 'gb']" @on-input="onInput" />
</template>

<script setup lang="ts">
import VueTelInput from 'vue-tel-input'
import 'vue-tel-input/vue-tel-input.css'

const phone = ref('')
const phoneObject = ref(null)

function onInput(formattedNumber, { number, valid, country }) {
  phoneObject.value = {
    formattedNumber,
    number,
    valid,
    country,
  }
}
</script>
