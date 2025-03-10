<template>
  <v-card
    elevation="1"
    class="rounded-lg h-100 cursor-pointer"
    @click="navigateToDetails"
  >
    <v-card-text class="pa-4 h-100">
      <!-- Header with Status -->
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center">
          <v-chip
            :color="getStatusColor(status)"
            :text-color="status === 'PENDING' ? 'default' : 'white'"
            size="small"
            class="font-weight-medium"
          >
            {{ getStatusText(status) }}
          </v-chip>
        </div>
        <div class="text-caption text-medium-emphasis">
          {{ formatDate(report_date) }}
        </div>
      </div>

      <!-- Note Type -->
      <div class="mb-2">
        <div class="text-subtitle-1 font-weight-bold">{{ note_type }}</div>
      </div>

      <!-- Description -->
      <p class="text-body-2 mb-4">{{ description }}</p>

      <!-- Location -->
      <div class="d-flex align-center mb-2">
        <v-icon size="small" color="primary" class="me-2"
          >mdi-map-marker</v-icon
        >
        <div class="d-flex flex-column flex-wrap">
          <span class="text-body-2 text-truncate" style="max-width: 250px">{{
            address
          }}</span>
          <div
            v-if="latitude && longitude"
            class="text-caption text-medium-emphasis"
          >
            {{ latitude }}, {{ longitude }}
          </div>
        </div>
      </div>

      <!-- Reporter -->
      <div class="d-flex align-center mb-2">
        <v-icon size="small" color="primary" class="me-2">mdi-account</v-icon>
        <div>
          <div class="d-flex align-center">
            <span class="text-body-2">
              {{ reportBy.firstname }} {{ reportBy.lastname }}</span
            >
          </div>
          <div class="text-caption text-medium-emphasis">
            <!-- {{ report_by.phone || report_by.email }} -->
          </div>
        </div>
      </div>

      <!-- Time -->
      <!-- <div class="d-flex align-center mt-2">
        <v-icon size="small" color="primary" class="me-2"
          >mdi-clock-outline</v-icon
        >
        <span class="text-body-2">{{ report_time }}</span>
      </div> -->
    </v-card-text>
  </v-card>
</template>

<script setup>
import { defineProps } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

const props = defineProps({
  noteId: {
    type: [Number, String],
    required: true,
  },
  note_type: {
    type: String,
    required: true,
  },
  status: {
    type: String,
    required: true,
  },
  description: {
    type: String,
    required: true,
  },
  report_date: {
    type: String,
    required: true,
  },
  report_time: {
    type: String,
    required: true,
  },
  address: {
    type: String,
    required: true,
  },
  reportBy: {
    default: null,
  },
  created_at: {
    type: String,
    required: true,
  },
  latitude: {
    type: Number,
    default: null,
  },
  longitude: {
    type: Number,
    default: null,
  },
});

const getStatusColor = (status) => {
  switch (status) {
    case "APPROVED":
      return "success";
    case "REJECTED":
      return "error";
    case "PENDING":
      return "warning";
    default:
      return "default";
  }
};

const getStatusText = (status) => {
  switch (status) {
    case "APPROVED":
      return "مقبولة";
    case "REJECTED":
      return "مرفوضة";
    case "PENDING":
      return "قيد المراجعة";
    default:
      return status;
  }
};

const formatDate = (date) => {
  if (!date) return "";
  return new Date(date).toLocaleDateString("ar-SA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

const getReporterType = (type) => {
  const types = {
    ADMIN: "مدير",
    USER: "مستخدم",
    SUPERVISOR: "مشرف",
    INSPECTOR: "مفتش",
    RESIDENT: "مقيم",
    VISITOR: "زائر",
    OTHER: "آخر",
  };
  return types[type] || type;
};

// Function to navigate to details page
const navigateToDetails = () => {
  router.push(`/notes/details/${props.noteId}`);
};
</script>

<style scoped>
.v-card {
  transition: transform 0.2s ease-in-out;
  cursor: pointer;
}

.v-card:hover {
  transform: translateY(-2px);
}
</style>
