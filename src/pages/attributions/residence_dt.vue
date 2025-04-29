<script setup lang="ts">
import { ref, computed, onMounted, watch } from "vue";
import { hasPermission } from "@core/utils/permissions";
import SearchableAutocomplete from "@/@core/components/global/SearchableAutocomplete.vue";
import Pagination from "@/@core/components/Pagination";
import { useRouter } from "vue-router";
import ResidenceCompany from "./components/rasidance_company.vue";
import ResidenceObserversCard from "./components/rasidance_Observers_card.vue";
import ResidenceSupervisorsCard from "./components/rasidance_supervisors_card.vue";

const router = useRouter();

// Static data matching the screenshot
const staticData = {
  company: {
    name: "شركة عبد الوهاب الراجحي",
    license_number: "6127782187",
    location: "عرض بالخريطة",
  },
  supervisors: [
    {
      id: 1,
      firstname: "محمد",
      lastname: "القحطاني عبدالله",
      role: { title: "مشرف" },
      sa_id: "92452 876 9988",
      avatar_url: null,
    },
    {
      id: 2,
      firstname: "محمد",
      lastname: "القحطاني عبدالله",
      role: { title: "مشرف" },
      sa_id: "92452 876 9988",
      avatar_url: null,
    },
  ],
  observers: [
    {
      id: 1,
      firstname: "راسي بن عبد الله",
      lastname: "سعيد",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
    {
      id: 2,
      firstname: "سعيد بن",
      lastname: "عبد الله",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
    {
      id: 3,
      firstname: "عبد الرحمن",
      lastname: "العريفي",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
    {
      id: 4,
      firstname: "عبدالله بن",
      lastname: "حربي",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
    {
      id: 5,
      firstname: "نصر الله كي",
      lastname: "شريف",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
    {
      id: 6,
      firstname: "نصر الله كي",
      lastname: "شريف",
      role: { title: "مراقب" },
      sa_id: "92452 876 9988",
      visit_count: 4,
      avatar_url: null,
    },
  ],
};

const isLoading = ref(false);
const dialog = ref(false);
const search = ref("");
const teamMembers = ref(staticData.observers);
const fieldTeamMembers = ref([]);

const filteredTeamMembers = computed(() => {
  if (!search.value) return teamMembers.value;

  const searchTerm = search.value.toLowerCase();
  return teamMembers.value.filter((member) => {
    return (
      member.firstname?.toLowerCase().includes(searchTerm) ||
      member.lastname?.toLowerCase().includes(searchTerm) ||
      member.sa_id?.toLowerCase().includes(searchTerm)
    );
  });
});

const breadcrumbItems = [
  {
    title: "الرئيسية",
    to: "/",
  },
  {
    title: "الإسنادات",
    to: "/attributions",
  },
  {
    title: "اسم المسكن",
    disabled: true,
  },
];

const inspectorsPagination = ref({
  currentPage: 3,
  itemsPerPage: 6,
  totalItems: staticData.observers.length,
});

function handlePageSizeChange(newSize) {
  inspectorsPagination.value.itemsPerPage = newSize;
  inspectorsPagination.value.currentPage = 1;
}
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-md-0 py-2 pt-0">
      <div>
        <v-breadcrumbs :items="breadcrumbItems" divider="<"></v-breadcrumbs>

        <v-container fluid class="mt-0 px-0">
          <!-- Company Section -->
          <ResidenceCompany :company="staticData.company" />

          <!-- Supervisors Section -->
          <div class="mt-8 bg-white rounded-lg pa-4">
            <h2 class="text-h6">المشرفون</h2>
            <v-row class=" ">
              <v-col
                v-for="supervisor in staticData.supervisors"
                :key="supervisor.id"
                cols="12"
                sm="6"
                md="6"
              >
                <ResidenceSupervisorsCard :user="supervisor" />
              </v-col>
              <v-col
                v-for="supervisor in staticData.supervisors"
                :key="supervisor.id"
                cols="12"
                sm="6"
                md="6"
              >
                <ResidenceSupervisorsCard :user="supervisor" />
              </v-col>
            </v-row>
          </div>

          <!-- Team Members Section -->
          <div class="mt-8">
            <div
              class="d-flex align-center justify-space-between bg-white rounded-lg pa-4"
            >
              <h6 class="text-h6 font-weight-medium">
                المراقبون ({{ teamMembers.length }} مراقبين)
              </h6>
              <div class="d-flex align-center gap-3">
                <v-text-field
                  v-model="search"
                  placeholder="البحث باسم الراقب أو رقم الهوية..."
                  rounded="lg"
                  clearable
                  hide-details
                  class="bg-light-gray-1"
                  style="width: 320px"
                  density="comfortable"
                >
                  <template v-slot:prepend-inner>
                    <v-icon color="medium-emphasis">mdi-magnify</v-icon>
                  </template>
                </v-text-field>

                <v-btn
                  v-if="hasPermission('teams.create')"
                  color="primary"
                  class="rounded-lg text-none"
                  prepend-icon="mdi-plus"
                >
                  إضافة مراقب
                </v-btn>
              </div>
            </div>

            <div class="mt-4">
              <v-row>
                <v-col
                  v-for="observer in filteredTeamMembers"
                  :key="observer.id"
                  cols="12"
                  sm="6"
                  md="4"
                >
                  <ResidenceObserversCard
                    :user="observer"
                    :visit-count="observer.visit_count"
                  />
                </v-col>
              </v-row>
            </div>
          </div>

          <!-- Pagination -->
          <div class="mt-4" v-if="teamMembers.length > 0">
            <Pagination
              :total-items="inspectorsPagination.totalItems"
              v-model:per-page="inspectorsPagination.itemsPerPage"
              v-model:page="inspectorsPagination.currentPage"
              @update:per-page="handlePageSizeChange"
            />
          </div>
        </v-container>
      </div>
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
.bg-light-gray-1 {
  background-color: rgb(250, 250, 250);
}

:deep(.v-field.v-field--rounded) {
  border-radius: 12px;
}

:deep(.v-text-field .v-field__input) {
  min-height: 44px;
  padding-top: 0;
  padding-bottom: 0;
}
</style>
