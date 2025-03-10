<template>
  <div class="corrections-list">
    <div class="d-flex justify-space-between align-center mb-6">
      <h2 class="text-h5">التصحيحات</h2>
      <div class="d-flex align-center gap-4">
        <div class="status-filter d-flex align-center gap-2">
          <v-chip
            v-for="filter in statusFilters"
            :key="filter.value"
            :color="filter.color"
            :variant="selectedStatus === filter.value ? 'elevated' : 'outlined'"
            @click="selectedStatus = filter.value"
            class="px-4"
          >
            {{ filter.label }}
            <span class="ms-2">({{ getStatusCount(filter.value) }})</span>
          </v-chip>
        </div>
      </div>
    </div>

    <v-row>
      <v-col
        v-for="correction in filteredCorrections"
        :key="correction.id"
        cols="12"
        sm="6"
        md="4"
        lg="4"
      >
        <CorrectSiteCard
          :correction="correction"
          :loading="loadingStates[correction.id]"
          @approve="handleApprove"
          @reject="handleReject"
        />
      </v-col>
    </v-row>

    <!-- No Results Message -->
    <div v-if="filteredCorrections.length === 0" class="text-center py-8">
      <v-icon size="48" color="grey-lighten-1" class="mb-4"
        >mdi-clipboard-text-search</v-icon
      >
      <div class="text-h6 text-grey-darken-1">لا توجد تصحيحات</div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import CorrectSiteCard from "@core/components/widgets/CorrectSiteCard.vue";
import axiosIns from "@/plugins/axios";
import { useToast } from "vue-toastification";

const toast = useToast();
const corrections = ref([]);
const loadingStates = ref({});
const selectedStatus = ref("all");

const statusFilters = [
  { label: "الكل", value: "all", color: "grey" },
  { label: "قيد الانتظار", value: "pending", color: "warning" },
  { label: "معتمد", value: "approved", color: "success" },
  { label: "مرفوض", value: "rejected", color: "error" },
];

const filteredCorrections = computed(() => {
  if (selectedStatus.value === "all") {
    return corrections.value;
  }
  return corrections.value.filter(
    (correction) => correction.status === selectedStatus.value
  );
});

const getStatusCount = (status) => {
  if (status === "all") {
    return corrections.value.length;
  }
  return corrections.value.filter((correction) => correction.status === status)
    .length;
};

const fetchCorrections = async () => {
  try {
    const response = await axiosIns.get("corrections");
    corrections.value = response.data.data;
    // Initialize loading states
    corrections.value.forEach((correction) => {
      loadingStates.value[correction.id] = false;
    });
  } catch (error) {
    console.error("Error fetching corrections:", error);
    toast.error("حدث خطأ أثناء جلب التصحيحات");
  }
};

const handleApprove = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axiosIns.post(`corrections/${id}/approve`);
    // Update the status in the local state
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "approved";
    }
    toast.success("تم اعتماد التصحيح بنجاح");
  } catch (error) {
    console.error("Error approving correction:", error);
    toast.error("حدث خطأ أثناء اعتماد التصحيح");
  } finally {
    loadingStates.value[id] = false;
  }
};

const handleReject = async (id) => {
  loadingStates.value[id] = true;
  try {
    await axiosIns.post(`corrections/${id}/reject`);
    // Update the status in the local state
    const correction = corrections.value.find((c) => c.id === id);
    if (correction) {
      correction.status = "rejected";
    }
    toast.success("تم رفض التصحيح بنجاح");
  } catch (error) {
    console.error("Error rejecting correction:", error);
    toast.error("حدث خطأ أثناء رفض التصحيح");
  } finally {
    loadingStates.value[id] = false;
  }
};

onMounted(() => {
  fetchCorrections();
});
</script>

<style lang="scss" scoped>
.corrections-list {
  padding: 1.5rem;
}

.status-filter {
  .v-chip {
    cursor: pointer;
    font-weight: 500;
  }
}
</style>
