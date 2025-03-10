<template>
  <v-card elevation="0" class="site-card rounded-lg">
    <v-card-text class="pa-4">
      <!-- Status Header -->
      <div class="status-header mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <!-- Status Badge -->
          <div class="status-badge d-flex align-center gap-2">
            <div class="status-icon">
              <v-icon
                v-if="data.status === 'approved'"
                color="success"
                size="20"
              >
                mdi-check-circle
              </v-icon>
              <v-icon
                v-else-if="data.status === 'rejected'"
                color="error"
                size="20"
              >
                mdi-close-circle
              </v-icon>
              <v-icon v-else color="warning" size="20">
                mdi-clock-outline
              </v-icon>
            </div>
            <span class="status-text" :class="statusTextClass">
              {{ statusText }}
            </span>
            <span class="camp-number">{{ data.camp_number }}</span>
          </div>

          <!-- Action Buttons -->
          <div class="action-buttons d-flex gap-2">
            <!-- Show Approve button if status is rejected or pending -->
            <v-btn
              v-if="['rejected', 'pending'].includes(data.status)"
              color="success"
              variant="outlined"
              size="small"
              class="text-none font-weight-medium px-4 rounded-pill"
              @click="handleApprove"
              :loading="loading"
            >
              <span class="me-2">
                {{ data.status === "rejected" ? "تراجع عن الرفض" : "اعتماد" }}
              </span>
              <v-icon size="small">mdi-check</v-icon>
            </v-btn>

            <!-- Show Reject button if status is approved or pending -->
            <v-btn
              v-if="['approved', 'pending'].includes(data.status)"
              color="error"
              variant="outlined"
              size="small"
              class="text-none font-weight-medium px-4 rounded-pill"
              @click="handleReject"
              :loading="loading"
            >
              <span class="me-2">
                {{ data.status === "approved" ? "تراجع عن الاعتماد" : "رفض" }}
              </span>
              <v-icon size="small">mdi-close</v-icon>
            </v-btn>
          </div>
        </div>
      </div>

      <!-- Info Grid -->
      <div class="site-info">
        <div class="info-grid">
          <template v-for="(item, index) in infoItems" :key="`info-${index}`">
            <div class="info-item" :data-full-width="item.fullWidth">
              <div class="info-label">
                <v-icon size="small" color="primary">{{ item.icon }}</v-icon>
                <span>{{ item.label }}</span>
              </div>
              <div class="info-value">{{ item.value }}</div>
              <div v-if="item.subValue" class="info-sub-value">
                {{ item.subValue }}
              </div>
            </div>
          </template>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script setup>
import { defineProps, defineEmits, computed, ref } from "vue";
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";

const appStore = useAppStore();
const isLoading = ref(false);

const props = defineProps({
  data: {
    type: Object,
    required: true,
    validator: (value) => {
      const requiredFields = [
        "id",
        "created_by",
        "camp_number",
        "square_number",
        "contact_number",
        "updated_at",
        "status",
      ];
      return requiredFields.every((field) => field in value);
    },
  },
  loading: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["refetch"]);

const handleApprove = async () => {
  try {
    isLoading.value = true;
    const newStatus = props.data.status === "rejected" ? "pending" : "approved";

    const response = await axios.put(`location-corrections/${props.data.id}`, {
      status: newStatus,
    });

    if (response.data.success === false) {
      throw new Error(response.data.message);
    }

    appStore.showSnackbar({
      message:
        newStatus === "approved"
          ? "تم اعتماد الموقع بنجاح"
          : "تم التراجع عن الرفض بنجاح",
      color: "success",
    });

    emit("refetch");
  } catch (error) {
    console.error("Error approving site:", error);
    appStore.showSnackbar({
      message: error.message || "حدث خطأ أثناء تحديث الموقع",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const handleReject = async () => {
  try {
    isLoading.value = true;
    const newStatus = props.data.status === "approved" ? "pending" : "rejected";

    const response = await axios.put(`location-corrections/${props.data.id}`, {
      status: newStatus,
    });

    if (response.data.success === false) {
      throw new Error(response.data.message);
    }

    appStore.showSnackbar({
      message:
        newStatus === "rejected"
          ? "تم رفض الموقع بنجاح"
          : "تم التراجع عن الاعتماد بنجاح",
      color: "success",
    });

    emit("refetch");
  } catch (error) {
    console.error("Error rejecting site:", error);
    appStore.showSnackbar({
      message: error.message || "حدث خطأ أثناء تحديث الموقع",
      color: "error",
    });
  } finally {
    isLoading.value = false;
  }
};

const formatDate = (dateString) => {
  if (!dateString) return "غير محدد";

  return new Date(dateString).toLocaleDateString("ar", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

const getFullName = computed(() => {
  const { firstname = "", lastname = "" } = props.data.created_by || {};
  return `${firstname} ${lastname}`.trim() || "غير محدد";
});

const statusText = computed(() => {
  const statusMap = {
    approved: "معتمد",
    rejected: "مرفوض",
    pending: "قيد الانتظار",
  };
  return statusMap[props.data.status] || "قيد الانتظار";
});

const statusTextClass = computed(() => {
  const classMap = {
    approved: "text-success",
    rejected: "text-error",
    pending: "text-warning",
  };
  return classMap[props.data.status] || "text-warning";
});

const infoItems = computed(() => [
  {
    icon: "mdi-account",
    label: "المنشئ",
    value: getFullName.value,
    fullWidth: true,
  },
  {
    icon: "mdi-tent",
    label: "رقم المخيم",
    value: props.data.camp_number || "غير محدد",
  },
  {
    icon: "mdi-square-outline",
    label: "رقم المربع",
    value: props.data.square_number || "غير محدد",
  },
  {
    icon: "mdi-phone",
    label: "رقم الاتصال",
    value: props.data.contact_number || "غير محدد",
  },
  {
    icon: "mdi-calendar",
    label: "تاريخ التحديث",
    value: formatDate(props.data.updated_at),
  },
]);
</script>

<style lang="scss" scoped>
.site-card {
  background-color: white;
  transition: all 0.3s ease;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  .status-header {
    border-bottom: 1px solid #eee;
    padding-bottom: 1rem;
  }

  .status-badge {
    .status-icon {
      display: flex;
      align-items: center;
    }

    .status-text {
      font-weight: 500;
      font-size: 0.9rem;
    }

    .camp-number {
      font-weight: 500;
      color: #666;
      margin-inline-start: 0.5rem;
      padding-inline-start: 0.5rem;
      border-inline-start: 2px solid #eee;
    }
  }

  .text-success {
    color: #65b669;
  }

  .text-error {
    color: #f53d6b;
  }

  .text-warning {
    color: #fb8c00;
  }

  .info-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    padding: 1rem;
    background-color: #f9f9f9;
    border-radius: 8px;
    margin-top: 1rem;
  }

  .info-item {
    &[data-full-width="true"] {
      grid-column: 1 / -1;
      border-bottom: 1px solid #eee;
      padding-bottom: 1rem;
      margin-bottom: 0.5rem;
    }

    .info-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      color: #666;
      margin-bottom: 0.25rem;
      font-size: 0.875rem;
    }

    .info-value {
      font-weight: 500;
      color: #333;
    }

    .info-sub-value {
      font-size: 0.75rem;
      color: #666;
      margin-top: 0.25rem;
    }
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }
}

.ga-1 {
  gap: 0.25rem;
}

.ga-2 {
  gap: 0.5rem;
}
</style>
