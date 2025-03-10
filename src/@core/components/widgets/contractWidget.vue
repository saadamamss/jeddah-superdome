<script setup>
import { computed, watch, onMounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const props = defineProps({
  id: {
    type: [Number, String],
    required: true,
  },
  title: {
    type: String,
    default: "",
  },
  created_at: {
    type: String,
    default: "",
  },
  updated_at: {
    type: String,
    default: "",
  },
});

// Debug watcher for props
watch(
  () => props,
  (newProps) => {
    console.log("Contract Widget Props:", {
      id: newProps.id,
      title: newProps.title,
      created_at: newProps.created_at,
      updated_at: newProps.updated_at,
    });
  },
  { immediate: true, deep: true }
);

// Debug on mount
onMounted(() => {
  console.log("Contract Widget Mounted with props:", props);
});

const handleClick = () => {
  router.push(`/contracts/${props.id}`);
};

const formatDate = (date) => {
  if (!date) {
    console.log("No date provided");
    return "";
  }
  try {
    console.log("Formatting date:", date);
    const dateObj = new Date(date);
    // First check if it's a valid date
    if (isNaN(dateObj.getTime())) {
      console.log("Invalid date:", date);
      return date;
    }

    // Format date in Arabic with Gregorian calendar
    const formatter = new Intl.DateTimeFormat("ar", {
      calendar: "gregory",
      day: "numeric",
      month: "short",
      year: "numeric",
      hour: "numeric",
      minute: "numeric",
      hour12: true,
    });

    const formattedDate = formatter.format(dateObj);
    console.log("Formatted date:", formattedDate);
    return formattedDate;
  } catch (error) {
    console.error("Error formatting date:", error);
    return date;
  }
};
</script>

<template>
  <VCard
    class="rounded-lg contract-card"
    style="box-shadow: 0px 4px 44px 4px rgba(0, 0, 0, 0.04)"
  >
    <v-card-text class="pa-3 pa-sm-4">
      <div class="d-flex justify-space-between align-center ga-2">
        <div>
          <span class="text-lg text-black font-weight-medium">{{ title }}</span>
        </div>
        <v-btn
          size="small"
          elevation="0"
          class="btn-card-link px-3 text-primary rounded-xl"
          @click.stop="handleClick"
        >
          <span class="font-weight-bold">عرض</span>
          <svg
            width="1.1rem"
            height="1.1rem"
            viewBox="0 0 16 16"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M6.38 12.2867C6.28667 12.2867 6.19333 12.2533 6.12 12.18C5.97333 12.0333 5.97333 11.7867 6.12 11.64L9.75333 8.00668L6.12 4.37334C5.97333 4.22668 5.97333 3.98001 6.12 3.83334C6.26667 3.68668 6.51333 3.68668 6.66 3.83334L10.6133 7.78668C10.76 7.93334 10.76 8.18001 10.6133 8.32668L6.66 12.28C6.56667 12.2533 6.47333 12.2867 6.38 12.2867Z"
              fill="#B69265"
            />
          </svg>
        </v-btn>
      </div>

      <div class="d-flex align-center justify-space-between mt-4">
        <div class="d-flex flex-column ga-2">
          <span class="text-caption text-medium-emphasis">تاريخ الإنشاء</span>
          <span class="text-body-2 text-grey-darken-1" dir="rtl">
            {{ created_at ? formatDate(created_at) : "No date" }}
          </span>
        </div>
        <div class="d-flex flex-column ga-2">
          <span class="text-caption text-medium-emphasis">آخر تحديث</span>
          <span class="text-body-2 text-grey-darken-1" dir="rtl">
            {{ updated_at ? formatDate(updated_at) : "No date" }}
          </span>
        </div>
      </div>
    </v-card-text>
  </VCard>
</template>

<style lang="scss" scoped>
.contract-card {
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0px 8px 48px 8px rgba(0, 0, 0, 0.08) !important;
  }
}

.btn-card-link {
  background-color: rgba(182, 146, 101, 0.1);
  color: #b69265;
  text-decoration: none;
  z-index: 1;

  &:hover {
    background-color: rgba(182, 146, 101, 0.2);
  }
}

.ga-2 {
  gap: 0.5rem;
}

.text-grey-darken-1 {
  color: rgba(0, 0, 0, 0.7);
}
</style>
