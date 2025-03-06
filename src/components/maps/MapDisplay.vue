<template>
  <div class="map-container" :style="{ height: height }">
    <div ref="mapRef" style="width: 100%; height: 100%"></div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from "vue";

const props = defineProps({
  latitude: {
    type: [Number, String],
    required: true,
  },
  longitude: {
    type: [Number, String],
    required: true,
  },
  height: {
    type: String,
    default: "400px",
  },
  zoom: {
    type: Number,
    default: 15,
  },
  readonly: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:latitude", "update:longitude"]);

const mapRef = ref(null);
let map = null;
let marker = null;

const initMap = () => {
  const lat =
    typeof props.latitude === "string"
      ? parseFloat(props.latitude)
      : props.latitude;
  const lng =
    typeof props.longitude === "string"
      ? parseFloat(props.longitude)
      : props.longitude;

  const position = { lat, lng };

  map = new google.maps.Map(mapRef.value, {
    center: position,
    zoom: props.zoom,
    streetViewControl: false,
    mapTypeControl: false,
  });

  marker = new google.maps.Marker({
    position,
    map,
    animation: google.maps.Animation.DROP,
    draggable: !props.readonly,
  });

  if (!props.readonly) {
    // Handle map clicks
    map.addListener("click", (e) => {
      const newPosition = {
        lat: e.latLng.lat(),
        lng: e.latLng.lng(),
      };
      marker.setPosition(newPosition);
      emit("update:latitude", newPosition.lat);
      emit("update:longitude", newPosition.lng);
    });

    // Handle marker drag
    marker.addListener("dragend", () => {
      const position = marker.getPosition();
      emit("update:latitude", position.lat());
      emit("update:longitude", position.lng());
    });
  }
};

const updateMarkerPosition = () => {
  if (!map || !marker) return;

  const lat =
    typeof props.latitude === "string"
      ? parseFloat(props.latitude)
      : props.latitude;
  const lng =
    typeof props.longitude === "string"
      ? parseFloat(props.longitude)
      : props.longitude;

  const position = { lat, lng };
  marker.setPosition(position);
  map.setCenter(position);
};

watch(
  () => [props.latitude, props.longitude],
  () => {
    updateMarkerPosition();
  }
);

onMounted(() => {
  if (window.google && window.google.maps) {
    initMap();
  } else {
    console.error("Google Maps not loaded");
  }
});
</script>

<style scoped>
.map-container {
  width: 100%;
  border-radius: 8px;
  overflow: hidden;
}
</style>
