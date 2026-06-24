<script setup lang="ts">
import { logger } from '@/@core/utils/logger'
import { useAppStore } from '@/stores/app.store'
import { rolesService } from '@/services/roles.service'
import { useI18n } from 'vue-i18n'
import type { Role } from '@/types'

const { t } = useI18n()

const props = defineProps<{
  modelValue: boolean
  role?: Role
}>()

// eslint-disable-next-line func-call-spacing
const emit = defineEmits<{
  (e: 'close'): void
  (e: 'update:modelValue', value: boolean): void
  (e: 'role-updated'): void
}>()

const appStore = useAppStore()
const isLoading = ref(false)

interface PermissionItemUI {
  title: string
  value: string
  checked: boolean
}

const permissions = ref<PermissionItemUI[]>([])
const isSubmitting = ref(false)
const dialogVisible = ref(false)
let abortController: AbortController | null = null

watch(
  () => props.modelValue,
  (val: boolean) => {
    dialogVisible.value = val
    if (val) {
      fetchPermissions()
    }
  },
)

watch(
  () => props.role,
  (newRole: Role | undefined) => {
    if (newRole && Object.keys(newRole).length > 0) {
      formState.name = newRole.name || ''
      formState.description = ''
      formState.permissions =
        newRole.permissions?.map((p: { name: string } | string) => (typeof p === 'object' ? p.name : p)) || []
    }
  },
  { immediate: true },
)

const updateDialog = (val: boolean) => {
  dialogVisible.value = val
  emit('update:modelValue', val)
}

const formState = reactive({
  name: '',
  description: '',
  permissions: [] as string[],
})

const fetchPermissions = async () => {
  if (abortController) abortController.abort()
  abortController = new AbortController()

  isLoading.value = true
  try {
    const response = await rolesService.getPermissions()
    permissions.value = response.data.map((permission) => {
      const isChecked = formState.permissions.includes(permission.key)
      return {
        title: permission.name,
        value: permission.key,
        checked: isChecked,
      }
    })
  } catch (error: unknown) {
    if (error instanceof DOMException && error.name === 'AbortError') return
    logger.error('Error fetching permissions:', error)
    appStore.showSnackbar({
      message: t('roles.dialog.loadPermissionsError'),
      color: 'error',
    })
  } finally {
    isLoading.value = false
  }
}

const togglePermission = (permissionId: string) => {
  const index = formState.permissions.indexOf(permissionId)
  if (index === -1) {
    formState.permissions.push(permissionId)
  } else {
    formState.permissions.splice(index, 1)
  }
}

const validateForm = () => {
  if (!formState.name) {
    appStore.showSnackbar({
      message: t('roles.dialog.nameRequired'),
      color: 'error',
    })
    return false
  }

  if (formState.permissions.length === 0) {
    appStore.showSnackbar({
      message: t('roles.dialog.permissionRequired'),
      color: 'error',
    })
    return false
  }

  return true
}

const handleSubmit = async () => {
  if (!validateForm()) return
  if (!props.role?.id) return

  isSubmitting.value = true
  try {
    await rolesService.update(props.role.id, {
      name: formState.name,
      description: formState.description,
      permissions: formState.permissions,
    })

    appStore.showSnackbar({
      message: t('roles.dialog.editSuccess'),
      color: 'success',
    })

    closeDialog()
    emit('role-updated')
  } catch (error: unknown) {
    logger.error('Error updating role:', error)
    const message = error instanceof Error ? error.message : t('roles.dialog.editError')
    appStore.showSnackbar({
      message,
      color: 'error',
    })
  } finally {
    isSubmitting.value = false
  }
}

const closeDialog = () => {
  emit('close')
  updateDialog(false)
}

onMounted(() => {
  if (props.modelValue) {
    fetchPermissions()
  }
})

onUnmounted(() => {
  if (abortController) abortController.abort()
})
</script>

<template>
  <v-dialog :model-value="modelValue" width="625" @update:model-value="updateDialog">
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 border-b pb-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">{{ t('roles.dialog.editTitle') }}</h2>
          <v-btn color="black" icon variant="text" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6 border-b pb-4">
        <v-form @submit.prevent="handleSubmit">
          <v-row>
            <v-col cols="12">
              <label class="d-inline-block mb-1 text-black">{{ t('roles.dialog.nameLabel') }}</label>
              <v-text-field
                v-model="formState.name"
                class="mb-1 rounded-lg"
                density="comfortable"
                :error-messages="!formState.name && isSubmitting ? t('roles.dialog.nameRequired') : ''"
                :placeholder="t('roles.dialog.namePlaceholder')"
                style="background-color: #f9f9f9"
                variant="outlined"
              />
            </v-col>
            <v-col cols="12">
              <label class="d-flex ga-1 align-center mb-1 text-black">
                <span>{{ t('roles.dialog.descriptionLabel') }}</span>
                <small style="color: #667178">{{ t('common.optional') }}</small>
              </label>
              <v-textarea
                v-model="formState.description"
                class="mb-1 rounded-lg"
                density="comfortable"
                :placeholder="t('roles.dialog.descriptionPlaceholder')"
                style="background-color: #ffffff"
                variant="outlined"
              />
            </v-col>
            <v-col cols="12">
              <label class="d-flex ga-1 align-center mb-1 text-black">
                {{ t('roles.dialog.permissionsLabel') }}
              </label>

              <div v-if="isLoading" class="d-flex justify-center pa-4">
                <v-progress-circular color="primary" indeterminate />
              </div>

              <div
                v-else
                class="permissions-container py-4 px-3 rounded-lg"
                :class="{
                  'error-border': formState.permissions.length === 0 && isSubmitting,
                }"
                style="max-height: 300px; overflow-y: auto"
              >
                <div v-if="permissions.length === 0" class="text-center py-4 text-medium-emphasis">
                  {{ t('roles.dialog.noPermissions') }}
                </div>

                <v-row v-else>
                  <v-col v-for="permission in permissions" :key="permission.value" class="py-0" cols="6">
                    <v-checkbox
                      v-model="permission.checked"
                      class="square-checkbox mb-1"
                      color="primary"
                      density="compact"
                      hide-details
                      :label="permission.title"
                      @update:model-value="togglePermission(permission.value)"
                    />
                  </v-col>
                </v-row>
              </div>

              <div v-if="formState.permissions.length === 0 && isSubmitting" class="text-error text-caption mt-1">
                {{ t('roles.dialog.permissionRequired') }}
              </div>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn
          color="black"
          :disabled="isSubmitting"
          rounded="lg"
          style="background-color: #f8f9f9"
          variant="text"
          @click="closeDialog"
        >
          <span class="d-inline-block px-4">{{ t('roles.dialog.cancel') }}</span>
        </v-btn>
        <v-spacer />
        <v-btn color="primary" :loading="isSubmitting" rounded="lg" variant="elevated" @click="handleSubmit">
          <span class="d-inline-block px-4">{{ t('roles.dialog.saveButton') }}</span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style scoped>
.error-border {
  border: 1px solid #ff5252;
}
.permissions-container {
  border: 1px solid rgba(0, 0, 0, 0.12);
}
/* Square checkbox styles */
:deep(.square-checkbox .v-selection-control__input .v-selection-control__input--density-default) {
  transform: translateX(2px);
}
:deep(.square-checkbox .v-selection-control__input .v-selection-control__control) {
  border-radius: 0 !important;
}
:deep(.square-checkbox .v-selection-control__input .mdi-checkbox-marked) {
  border-radius: 0 !important;
}
:deep(.square-checkbox.v-checkbox .v-selection-control) {
  min-height: 32px;
}
</style>
