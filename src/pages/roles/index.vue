<script setup>
// import { useAppStore } from "@/stores/app";
import RoleCard from "./components/RoleCard.vue";
import AddNewRoleDialog from "./components/AddNewRoleDialog.vue";
import DeleteRoleConfirmationDialog from "./components/DeleteRoleConfirmationDialog.vue";

// const appStore = useAppStore();
const isLoading = ref(true);
const isAddingDialog = ref(false);
const isDeletingDialog = ref(false);
const currentRoleId = ref(null);

const handleDeleteRole = (roleId) => {
  currentRoleId.value = roleId;
  isDeletingDialog.value = true;
};

const closeDeletingDialog = () => {
  currentRoleId.value = null;
  isDeletingDialog.value = false;
};

onMounted(() => {
  setTimeout(() => {
    isLoading.value = false;
  }, 3000);
});
</script>

<template>
  <div>
    <v-container fluid class="px-0 px-sm-4 px-xl-0 py-2">
      <VCard elevation="0" class="rounded-lg px-4 bg-light-white">
        <v-card-title class="pt-4 pb-6">
          <div
            class="d-flex flex-column flex-sm-row align-sm-center justify-sm-space-between ga-4 border-b pb-4"
          >
            <h2 class="text-h5 font-weight-bold mb-4">
              إدارة الأدوار والصلاحيات
            </h2>
            <div class="d-flex align-center ga-2">
              <VBtn
                height="48"
                elevation="0"
                class="rounded-lg"
                append-icon="mdi-plus"
                @click="isAddingDialog = true"
              >
                إضافة دور جديد
              </VBtn>
            </div>
          </div>

          <v-row v-if="isLoading" class="py-4">
            <v-col v-for="item in 4" :key="item" cols="6" sm="6" md="4">
              <VCard>
                <v-card-text class="px-1 py-2">
                  <v-skeleton-loader
                    class="mx-auto"
                    elevation="0"
                    max-width="400"
                    type="list-item-two-line, table-heading"
                  ></v-skeleton-loader>
                </v-card-text>
              </VCard>
            </v-col>
          </v-row>
          <v-row v-else class="mt-3">
            <v-col
              v-for="item in 4"
              :key="item"
              cols="12"
              sm="6"
              md="4"
              lg="4"
              class="pa-3"
            >
              <RoleCard @delete-role="handleDeleteRole" />
            </v-col>
          </v-row>

          <Pagination
            :total-items="2300"
            v-model:per-page="perPage"
            v-model:page="currentPage"
          />
        </v-card-title>
      </VCard>

      <!-- Add New Role Modal -->
      <AddNewRoleDialog
        v-model="isAddingDialog"
        @close="isAddingDialog = false"
      />

      <!-- Add New Role Modal -->
      <DeleteRoleConfirmationDialog
        v-model="isDeletingDialog"
        :current-role-id="currentRoleId"
        @close="closeDeletingDialog"
      />
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
// @use "@core/scss/template/libs/apex-chart.scss";
.grid {
  flex-wrap: wrap;

  &__item {
    // width: 100%;/

    @media (min-width: 960px) {
      width: calc((100% - 4px) / 5);
    }

    @media (min-width: 600px) and (max-width: 959px) {
      width: calc((100% - 4px) / 2);
    }

    @media (max-width: 599px) {
      width: 100%;
    }
  }
}
</style>
