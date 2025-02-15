<script setup>
import { useAppStore } from "@/stores/app";
import AppDateTimePicker from "@/@core/components/AppDateTimePicker";
import TableColumnsFilter from "@/@core/components/pages/TableColumnsFilter";
import smoke from "@/assets/images/smoke.png";
import plant1 from "@/assets/images/plant1.png";
import plant2 from "@/assets/images/plant2.png";
import plant3 from "@/assets/images/plant3.png";
import car1 from "@/assets/images/car1.png";
import car2 from "@/assets/images/car2.png";
import car3 from "@/assets/images/car3.png";
import equipment from "@/assets/images/equipment.png";
const appStore = useAppStore();

const search = ref("");
const filter = ref(null);
const selectedDrownStatus = ref(0);
const dateRange = ref({
  start: null,
  end: null,
});

// Dialogs
const viewDialog = ref(false);
const selectedDrone = ref(null);

// Pagination
const rowPerPage = ref(10);
const currentPage = ref(1);
const totalItems = ref(0);
const isFilterSidebar = ref(false);
const filters = ref({
  date: [],
  performance_percentage: [20, 80],
});

const headers = ref([
  { display: true, title: "نموذج ID", align: "center", key: "id" },
  { display: true, title: "التاريخ", align: "center", key: "date" },
  { display: true, title: "النماذج", align: "center", key: "obstacles" },
  { display: true, title: " خط الطول  ", align: "center", key: "longitude" },
  { display: true, title: " خط العرض  ", align: "center", key: "latitude" },
  { display: true, title: "الحالة  ", align: "center", key: "status" },
  { display: true, title: "الإجراءات", align: "center", key: "actions" },
]);

const updatedHeaders = computed(
  () => headers.value?.filter((h) => h.display) || []
);

const droneData = [
  {
    id: 1,
    date: "2024-01-20 10:30",
    obstacles: "حريق",
    longitude: "N ’22’1’2",
    latitude: "E ’22’1’2",
    status: 0,
    images: [
      smoke
    ],
  },
  {
    id: 2,
    date: "2024-01-20 11:15",
    obstacles: "نباتات",
    longitude: "N ’22’1’2",
    latitude: "E ’22’1’2",
    status: 1,
    images: [
      plant1,
      plant2,
      plant3
    ],
  },
  {
    id: 3,
    date: "2024-01-20 11:15",
    obstacles: "مركبات",
    longitude: "N ’22’1’2",
    latitude: "E ’22’1’2",
    status: 1,
    images: [
      car1,
      car2,
      car3
    ],
  },
  {
    id: 3,
    date: "2024-01-20 11:15",
    obstacles: "معدات",
    longitude: "N ’22’1’2",
    latitude: "E ’22’1’2",
    status: 1,
    images: [
      equipment
    ],
  },
];

const toggleFilterSidebar = () => {
  isFilterSidebar.value = !isFilterSidebar.value;
};

const virtualDrones = computed(() => {
  const filtered = [...Array(100).keys()].map((i) => {
    const drone = { ...droneData[i % droneData.length] };
    drone.id = i + 98778;
    drone.date = `2024-01-${String(i + 1).padStart(2, "0")} 10:30`;
    return drone;
  });

  let result = filtered;

  // Filter by status if selected
  if (filter.value) {
    result = result.filter((drone) => {
      if (filter.value.value === 1) return drone.status === "فعاله";
      if (filter.value.value === 2) return drone.status === "عطل";
      return true;
    });
  }

  // Filter by date range if set
  if (dateRange.value.start && dateRange.value.end) {
    const start = new Date(dateRange.value.start);
    const end = new Date(dateRange.value.end);
    result = result.filter((drone) => {
      const droneDate = new Date(drone.date);
      return droneDate >= start && droneDate <= end;
    });
  }

  return result;
});

// Pagination computed
const paginatedDrones = computed(() => {
  const start = (currentPage.value - 1) * rowPerPage.value;
  const end = start + rowPerPage.value;
  totalItems.value = virtualDrones.value.length;
  return virtualDrones.value.slice(start, end);
});

const openView = (item) => {
  selectedDrone.value = item;
  viewDialog.value = true;
};
</script>

<template>
  <div>
    <VCard
      class="w-100 py-3 px-3 border"
      elevation="0"
      rounded="0"
      style="overflow: initial; z-index: initial"
    >
      <div
        class="d-flex align-center justify-space-between ma-4 flex-wrap gap-4"
      >
        <div class="d-flex align-center gap-4 flex-wrap">
          <div class="d-flex align-center gap-2">
            <span class="font-weight-bold">تصفية:</span>
            <v-select
              v-model="selectedDrownStatus"
              :items="[
                { title: 'الكل', value: 0 },
                { title: 'تحت المعالجة', value: 1 },
                { title: 'تمت المعالجة', value: 2 },
              ]"
              item-title="title"
              item-value="value"
              return-object
              rounded="lg"
              placeholder="تصفية النتائج"
              outlined
              dense
              style="min-width: 155px"
            />
          </div>

          <div>
            <AppDateTimePicker
              v-model="filters.date"
              format="LLL d, yyyy"
              style="--dp-input-padding: 10px 40px 10px 20px"
            />
          </div>
        </div>

        <div class="d-flex align-center flex-wrap gap-2">
          <v-text-field
            v-model="search"
            label="بحث"
            rounded="lg"
            prepend-inner-icon="mdi-magnify"
            clearable
            outlined
            dense
            height="43"
            style="min-width: 260px"
          ></v-text-field>
          <v-btn
            color="primary"
            variant="outlined"
            rounded="lg"
            prepend-icon="mynaui:filter-solid"
            class="px-4"
            @click="toggleFilterSidebar"
            :active="isFilterSidebar"
          >
            تصفية
          </v-btn>
          <v-menu>
            <template v-slot:activator="{ props }">
              <v-btn
                color="primary"
                variant="outlined"
                rounded="lg"
                v-bind="props"
                append-icon="solar:alt-arrow-down-line-duotone"
                class="px-4"
              >
                تصدير كـ
              </v-btn>
            </template>
            <v-list>
              <v-list-item href="#">
                <v-list-item-title>تصدير كملف PDF</v-list-item-title>
              </v-list-item>
              <v-list-item href="#">
                <v-list-item-title>تصدير كملف Excel</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </div>
      </div>
    </VCard>
    <div class="w-100 d-flex">
      <div class="w-100">
        <v-data-table
          :headers="updatedHeaders"
          :items="paginatedDrones"
          :items-per-page="rowPerPage"
          :page="currentPage"
          :search="search"
          :total-items="totalItems"
          @update:page="currentPage = $event"
          @update:items-per-page="rowPerPage = $event"
          hide-default-footer
        >
          <template #item.obstacles="{ item }">
            <div
              class="d-inline-flex align-center gap-2 pa-1 px-2 rounded-pill w-max-content text-no-wrap bg-light-red"
            >
              <v-icon
                color="dark-red"
                icon="mdi-circle"
                size="x-small"
              ></v-icon>
              <span class="text-dark-red font-weight-medium">
                {{ item.obstacles }}
              </span>
            </div>
          </template>

          <template #item.status="{ item }">
            <v-btn variant="text">
              <span v-if="item.status" class="text-dark-green">
                تمت المعالجة
              </span>
              <span v-else class="text-dark-yellow"> تحت المعالجة </span>
            </v-btn>
          </template>

          <template #item.actions="{ item }">
            <v-btn
              color="primary"
              @click="openView(item)"
              rounded="lg"
              class="px-6"
              >عرض</v-btn
            >
          </template>

          <template #bottom>
            <v-container class="py-0">
              <v-row justify="center">
                <v-col cols="8">
                  <v-container class="max-width">
                    <v-pagination
                      v-model="currentPage"
                      tile
                      :length="10"
                      :total-visible="5"
                      class="my-4"
                    >
                    </v-pagination>
                  </v-container>
                </v-col>
              </v-row>
            </v-container>
          </template>
        </v-data-table>
      </div>
      <div v-if="isFilterSidebar" style="width: 260px" class="bg-white border">
        <div class="advances-filter-bar px-4 pb-2 d-flex flex-column gap-4">
          <TableColumnsFilter
            :headers="headers"
            @update="($event) => (headers = $event)"
          />

          <div>
            <h2 class="text-subtitle-1 mb-8">نسبة الأداء:</h2>
            <v-range-slider
              v-model="filters.performance_percentage"
              step="10"
              thumb-label="always"
            >
              <template v-slot:thumb-label="{ modelValue }">
                {{ modelValue }}%
              </template>
            </v-range-slider>
          </div>

          <div class="d-flex justify-end mt-3 px-4">
            <v-btn block color="primary" rounded="lg">تطبيق</v-btn>
          </div>
        </div>
      </div>
    </div>

    <!-- View Dialog -->
    <v-dialog
      v-model="viewDialog"
      :width="$vuetify.display.lgAndUp ? 1070 : '90%'"
      :height="580"
    >
      <v-card height="100%" rounded="lg">
        <v-card-title class="d-flex justify-space-between align-center pa-6">
          <h2 class="text-h6 font-weight-bold">تفاصيل النموذج</h2>
          <v-spacer></v-spacer>
          <v-btn icon variant="text" @click="viewDialog = false">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-4" v-if="selectedDrone">
          <v-container fluid>
            <!-- Add your view content here -->
            <v-row>
              <v-col cols="12" md="6">
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  معرف الدرون:
                </div>
                <div>{{ selectedDrone.id }} </div>

              </v-col>
              <v-col cols="12" md="6">
                <div class="text-subtitle-1 font-weight-bold mb-2">
                  التاريخ:
                </div>
                <div>{{ selectedDrone.date }}</div>
              </v-col>
              <!-- Add more details as needed -->
            </v-row>
            <!-- Drone Images -->
            <v-row class="mt-4">
              <v-col cols="12">
                <div class="text-subtitle-1 font-weight-bold mb-2">صور الدرون:</div>
                <div class="d-flex flex-wrap gap-4 ">
                  <v-img
                    v-for="(image, index) in selectedDrone.images"
                    :key="index"
                    :src="image"
                    class="rounded-lg"
                    max-height="350"
                    max-width="500"
                  ></v-img>
                </div>
              </v-col>
            </v-row>
          </v-container>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
</template>

<style lang="scss">
.bold-dots {
  font-weight: bold;
  font-size: 1.2rem;
  color: #000 !important;
}

thead {
  tr {
    background-color: #f2f4f7;
    color: #667085;
  }
}
</style>
