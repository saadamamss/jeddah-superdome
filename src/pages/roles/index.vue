<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import Pagination from '@core/components/Pagination.vue'
import RoleCard from './components/RoleCard.vue'
import AddNewRoleDialog from './components/AddNewRoleDialog.vue'
import EditRoleDialog from './components/EditRoleDialog.vue'
import DeleteRoleConfirmationDialog from './components/DeleteRoleConfirmationDialog.vue'
import { useAppStore } from '@/stores/app.store'
import { can } from '@layouts/plugins/casl'
import { rolesService } from '@/services/roles.service'
import { useI18n } from 'vue-i18n'
import { useTablePage } from '@/composables/useTablePage'
import { useConfirmDialog } from '@/composables/useConfirmDialog'
import type { Role } from '@/types'

const { t } = useI18n()
const appStore = useAppStore()

const {
  items: roles,
  isLoading,
  currentPage,
  perPage,
  totalItems,
  fetchData: fetchRoles,
} = useTablePage((params) => rolesService.getAll(params), {
  entityName: t('roles.breadcrumbTitle'),
  fetchOnMount: false,
})

const isAddingDialog = ref(false)
const isEditingDialog = ref(false)
const currentRole = ref<Role | undefined>(undefined)

const {
  isOpen: isDeletingDialog,
  selectedId: currentRoleId,
  isProcessing,
  open: handleDeleteRole,
  close: closeDeletingDialog,
  confirm,
} = useConfirmDialog({
  onConfirm: async (id: number) => {
    try {
      await rolesService.delete(id)
      appStore.showSnackbar({ message: t('roles.messages.deleteSuccess'), color: 'success' })
      fetchRoles()
    } catch (error: unknown) {
      logger.error('Error deleting role:', error)
      const message = error instanceof Error ? error.message : t('roles.messages.deleteError')
      appStore.showSnackbar({ message, color: 'error' })
      throw error
    }
  },
})

const handleEditRole = (role: Role) => {
  currentRole.value = role
  isEditingDialog.value = true
}

const closeEditingDialog = () => {
  currentRole.value = undefined
  isEditingDialog.value = false
}

const handleRoleAdded = () => {
  fetchRoles()
}

const handleRoleUpdated = () => {
  fetchRoles()
}

onMounted(() => {
  fetchRoles()
})
</script>

<template>
  <div>
    <v-container class="px-0 px-sm-4 px-xl-0 py-2 pt-0" fluid>
      <div class="mb-2">
        <v-breadcrumbs
          divider=">"
          :items="[
            {
              title: t('common.home'),
              disabled: false,
              to: '/',
            },
            {
              title: t('roles.breadcrumbTitle'),
              disabled: true,
            },
          ]"
        >
          <template #prepend>
            <svg fill="none" height="20" viewBox="0 0 20 20" width="20" xmlns="http://www.w3.org/2000/svg">
              <path
                d="M5.62519 16.0419H14.3752C15.2957 16.0419 16.0419 15.2957 16.0419 14.3752V8.12519L10.0002 3.95853L3.95853 8.12519V14.3752C3.95853 15.2957 4.70472 16.0419 5.62519 16.0419Z"
                stroke="#25343E"
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="1.5"
              />
              <path
                d="M8.12468 13.1244C8.12468 12.2039 8.87088 11.4577 9.79135 11.4577H10.208C11.1285 11.4577 11.8747 12.2039 11.8747 13.1244V16.0411H8.12468V13.1244Z"
                stroke="#25343E"
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="1.5"
              />
            </svg>
          </template>
        </v-breadcrumbs>
      </div>
      <VCard class="rounded-lg px-4 bg-light-white" elevation="0">
        <v-card-title class="pt-4 pb-6">
          <div class="d-flex flex-column flex-sm-row align-sm-center justify-sm-space-between ga-4 border-b pb-4">
            <h2 class="text-h5 font-weight-bold mb-4">
              {{ t('roles.breadcrumbTitle') }}
            </h2>
            <div class="d-flex align-center ga-2">
              <VBtn
                v-if="$can('create', 'roles')"
                append-icon="mdi-plus"
                class="rounded-lg"
                elevation="0"
                height="48"
                @click="isAddingDialog = true"
              >
                {{ t('roles.addRole') }}
              </VBtn>
            </div>
          </div>

          <v-row v-if="isLoading" class="py-4">
            <v-col v-for="item in 4" :key="item" cols="6" md="4" sm="6">
              <VCard>
                <v-card-text class="px-1 py-2">
                  <v-skeleton-loader
                    class="mx-auto"
                    elevation="0"
                    max-width="400"
                    type="list-item-two-line, table-heading"
                  />
                </v-card-text>
              </VCard>
            </v-col>
          </v-row>
          <v-row v-else-if="roles.length > 0" class="mt-3">
            <v-col v-for="role in roles" :key="String(role.id)" class="pa-3" cols="12" lg="4" md="4" sm="6">
              <RoleCard
                :can-delete="can('delete', 'roles')"
                :can-edit="can('edit', 'roles')"
                :role="role"
                @delete-role="handleDeleteRole"
                @edit-role="handleEditRole"
              />
            </v-col>
          </v-row>
          <v-row v-else class="py-4">
            <v-col cols="12">
              <div class="text-center py-8">
                <h3 class="text-lg font-weight-medium">{{ t('roles.emptyTitle') }}</h3>
                <p class="text-medium-emphasis mt-2">
                  {{ t('roles.emptySubtitle') }}
                </p>
              </div>
            </v-col>
          </v-row>

          <!-- pagination -->
          <Pagination
            v-if="totalItems > 0"
            v-model:page="currentPage"
            v-model:per-page="perPage"
            :total-items="totalItems"
          />
        </v-card-title>
      </VCard>

      <!-- Add New Role Dialog -->
      <AddNewRoleDialog v-model="isAddingDialog" @close="isAddingDialog = false" @role-added="handleRoleAdded" />

      <!-- Edit Role Dialog -->
      <EditRoleDialog
        v-model="isEditingDialog"
        :role="currentRole"
        @close="closeEditingDialog"
        @role-updated="handleRoleUpdated"
      />

      <!-- Delete Role Confirmation Dialog -->
      <DeleteRoleConfirmationDialog
        v-model="isDeletingDialog"
        :current-role-id="currentRoleId"
        :is-deleting="isProcessing"
        @close="closeDeletingDialog"
        @delete="confirm"
      />
    </v-container>
  </div>
</template>

<style lang="scss" scoped>
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
