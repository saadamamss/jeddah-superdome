import { onMounted, onUnmounted } from 'vue'
import AOS from 'aos'
import 'aos/dist/aos.css'

export function useAos() {
  onMounted(() => {
    setTimeout(() => {
      AOS.init({
        duration: 800,
        easing: 'ease-in-out',
        once: true,
        offset: 100,
        delay: 100,
      })
    }, 100)
  })

  onUnmounted(() => {
    AOS.refresh()
  })
}
