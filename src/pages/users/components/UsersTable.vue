<script setup lang="ts">
import AddNewUserDialog from './AddNewUserDialog.vue'
import { useConfirmDialog } from '@/composables/useConfirmDialog'

const isLoading = ref(false)

// Dialogs
const isAddingDialog = ref(false)
const editDialog = ref(false)
const selectedTeam = ref(null)

const {
  isOpen: deleteDialog,
  close,
  confirm,
} = useConfirmDialog({
  onConfirm: async () => {
    /* no-op: mock data */
  },
})

// Pagination
const rowPerPage = ref(10)
const currentPage = ref(1)

const page = ref(1)

const headers = [
  { title: 'ID رقم', align: 'center', key: 'id' },
  { title: 'اسم الموطف', align: 'center', key: 'name' },
  { title: 'الصلاحية', align: 'center', key: 'permission' },
  { title: 'تاريخ التعيين', align: 'center', key: 'date' },
  { title: 'الدور', align: 'center', key: 'role' },
  { title: 'الإجراءات', align: 'end', key: 'actions' },
]

const droneData = [
  {
    id: 1,
    name: 'كريم محمد احمد',
    permission: 'مدير الدرون',
    date: '2024-01-20 10:30',
    role: 'هذا النص يمكن استبداله',
  },
  {
    id: 2,
    name: 'محمود فتحي',
    permission: 'مدير الصيانه',
    date: '2024-01-20 11:15',
    role: 'هذا النص يمكن استبداله',
  },
]

const virtualDrones = computed(() => {
  const filtered = [...Array(100).keys()].map((i) => {
    const drone = { ...droneData[i % droneData.length] }
    drone.id = i + 98778
    drone.registration = `DRN${String(i + 1).padStart(3, '0')}`
    return drone
  })

  return filtered
})

// Pagination computed
const paginatedDrones = computed(() => {
  const start = (currentPage.value - 1) * rowPerPage.value
  const end = start + rowPerPage.value
  return virtualDrones.value.slice(start, end)
})

const totalItems = computed(() => virtualDrones.value.length)
</script>

<template>
  <v-skeleton-loader v-if="isLoading" class="mx-auto" elevation="0" max-width="100%" type="table" />
  <VCard v-else elevation="0">
    <v-data-table
      :headers="headers"
      hide-default-footer
      :items="paginatedDrones"
      :items-per-page="rowPerPage"
      :page="currentPage"
      :search="search"
      :total-items="totalItems"
      @update:items-per-page="rowPerPage = $event"
      @update:page="currentPage = $event"
    >
      <template #top>
        <div class="d-flex align-center justify-space-between ma-4">
          <div class="d-flex align-center gap-2">
            <div>التصفيه:</div>
            <v-select
              v-model="selectedTeam"
              item-title="title"
              :items="[{ title: 'الكل' }, { title: 'فريق أ' }, { title: 'فريق ب' }, { title: 'فريق ج' }]"
              placeholder="الفريق"
              return-object
              style="min-width: 277px"
            />
          </div>

          <div class="d-flex align-center gap-2">
            <v-text-field
              v-model="search"
              clearable
              label="بحث"
              prepend-inner-icon="mdi-magnify"
              style="min-width: 306px"
            />
            <VBtn variant="outlined" prepend-icon="mdi-printer" @click="printTable"> طباعه </VBtn>
            <!-- <VBtn color="primary" @click="isAddingDialog = true">
              إضافة مشغل جديد
            </VBtn> -->
            <v-btn color="primary" rounded="lg" @click="isAddingDialog = true"> إضافة مستخدم جديد </v-btn>
          </div>
        </div>
      </template>
      <template #item.status="{ item }">
        <div
          class="d-inline-flex align-center gap-2 pa-1 px-2 rounded-pill w-max-content text-no-wrap"
          :class="item.status === 'تم الصيانتها' ? 'bg-light-green' : 'bg-light-yellow'"
        >
          <div class="ps-2 mt-n3">
            <v-badge v-if="item.status === 'تم الصيانتها'" color="success" dot />
            <v-badge v-else color="warning" dot />
          </div>
          <span
            class="text-sm font-weight-semibold"
            :class="item.status === 'تم الصيانتها' ? 'text-dark-green' : 'text-dark'"
          >
            {{ item.status }}
          </span>
        </div>
      </template>

      <!-- eslint-disable-next-line vue/no-unused-vars -->
      <template #item.actions="{ item: _item }">
        <v-btn class="" variant="plain">
          <v-icon>basil:edit-outline</v-icon>
        </v-btn>
        <v-btn class="" color="dark-red" variant="plain">
          <v-icon>mdi-delete</v-icon>
        </v-btn>
      </template>

      <template #bottom>
        <div class="text-center">
          <v-container>
            <v-row justify="center">
              <v-col cols="8">
                <v-container class="max-width">
                  <v-pagination v-model="page" class="my-4" :length="10" tile :total-visible="5" />
                </v-container>
              </v-col>
            </v-row>
          </v-container>
        </div>
      </template>
    </v-data-table>

    <AddNewUserDialog v-model="isAddingDialog" @close="isAddingDialog = false" />

    <!-- Edit Dialog -->
    <v-dialog v-model="editDialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">تعديل </span>
        </v-card-title>
        <v-card-text>
          <v-container>
            <!-- Add form fields here -->
          </v-container>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="blue-darken-1" variant="text" @click="editDialog = false"> Cancel </v-btn>
          <v-btn color="blue-darken-1" variant="text" @click="editDialog = false"> Save </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Dialog -->
    <v-dialog v-model="deleteDialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">Delete Drone</span>
        </v-card-title>
        <v-card-text> Are you sure you want to delete this drone? </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="blue-darken-1" variant="text" @click="close"> Cancel </v-btn>
          <v-btn color="red-darken-1" variant="text" @click="confirm"> Delete </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </VCard>
</template>

<style lang="scss">
.bold-dots {
  font-weight: bold;
  font-size: 1.2rem;
  color: #000 !important;
}
</style>
