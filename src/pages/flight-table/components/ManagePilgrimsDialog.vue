<template>
  <v-dialog v-model="dialog" max-width="800" height="90vh" persistent content-class="manage-pilgrims-dialog">
    <v-card rounded="lg" class="d-flex flex-column">
      <v-card-title class="bg-primary_2 py-3 rounded-lg flex-shrink-0">
        <div class="d-flex align-center justify-space-between">
          <div>
            <h5 class="mb-1">
              {{ t('flights.pilgrimAssignment.title', { flightNumber: flight?.flightNumber || flightId }) }}
            </h5>
            <p v-if="flight" class="text-sm text-secondary mb-0">
              {{
                t('flights.pilgrimAssignment.flightInfo', {
                  destination: flight.destination,
                  date: flight.date,
                  count: assignedTotal,
                  capacity: flight.capacity || '—',
                })
              }}
            </p>
          </div>
          <button @click="dialog = false">
            <v-icon size="sm">mdi-close</v-icon>
          </button>
        </div>
      </v-card-title>

      <v-card-text>
        <v-tabs v-model="tab" class="px-4 pt-2 flex-shrink-0 tabs" color="primary" grow>
          <v-tab value="assign">{{ t('flights.pilgrimAssignment.assignTab') }}</v-tab>
          <v-tab value="assigned">{{ t('flights.pilgrimAssignment.assignedTab') }}</v-tab>
        </v-tabs>

        <v-tabs-window v-model="tab" style="flex: 1; overflow: hidden">
          <!-- ═══ Assign Tab ═══ -->
          <v-tabs-window-item value="assign" style="height: 100%; overflow: hidden">
            <div class="d-flex flex-column" style="height: 100%">
              <div style="flex: 1; overflow-y: auto; padding: 16px 24px 0">
                <v-row v-if="availableLoading" class="justify-center py-10">
                  <v-progress-circular indeterminate color="primary" />
                </v-row>

                <template v-else>
                  <p v-if="isCapacityFull" class="text-error text-sm mb-2">
                    {{ t('flights.pilgrimAssignment.capacityFull') }}
                  </p>

                  <v-text-field
                    v-model="searchAvailableQuery"
                    :placeholder="t('flights.pilgrimAssignment.searchAvailable')"
                    density="compact"
                    hide-details
                    rounded="lg"
                    clearable
                    class="mb-3"
                  />

                  <div v-if="availablePilgrims.length > 0" class="pilgrim-grid">
                    <div
                      v-for="p in availablePilgrims"
                      :key="p.id"
                      class="pilgrim-card"
                      :class="{ 'pilgrim-card-selected': selectedPilgrimIds.includes(p.id) }"
                      @click="togglePilgrim(p.id)"
                    >
                      <v-icon
                        class="pilgrim-check-icon"
                        :color="selectedPilgrimIds.includes(p.id) ? 'primary' : '#bdbdbd'"
                        size="small"
                      >
                        {{ selectedPilgrimIds.includes(p.id) ? 'mdi-checkbox-marked' : 'mdi-checkbox-blank-outline' }}
                      </v-icon>
                      <div class="pilgrim-info">
                        <span class="pilgrim-name">{{ p.name }}</span>
                        <span class="pilgrim-identity">{{ p.identity }}</span>
                      </div>
                    </div>
                  </div>

                  <p
                    v-else-if="availableTotal === 0 && !searchAvailableQuery"
                    class="text-sm text-medium-emphasis py-6 text-center"
                  >
                    {{ t('flights.pilgrimAssignment.noAvailable') }}
                  </p>
                  <p v-else class="text-sm text-medium-emphasis py-6 text-center">
                    {{ t('common.noResults') }}
                  </p>

                  <v-btn
                    color="primary"
                    :disabled="selectedPilgrimIds.length === 0 || isAssigning || isCapacityFull"
                    :loading="isAssigning"
                    variant="flat"
                    class="rounded-lg mt-2 mb-4"
                    @click="handleAssign"
                  >
                    {{ t('flights.pilgrimAssignment.assignSelected', { count: selectedPilgrimIds.length }) }}
                  </v-btn>
                </template>
              </div>

              <div v-if="availableTotalPages > 1" class="flex-shrink-0 px-6 pb-2">
                <Pagination
                  v-model:page="availablePage"
                  v-model:per-page="availablePerPage"
                  :total-items="availableTotal"
                />
              </div>
            </div>
          </v-tabs-window-item>

          <!-- ═══ Assigned Tab ═══ -->
          <v-tabs-window-item value="assigned" style="height: 100%; overflow: hidden">
            <div class="d-flex flex-column" style="height: 100%">
              <div style="flex: 1; overflow-y: auto; padding: 16px 24px 0">
                <v-row v-if="assignedLoading" class="justify-center py-10">
                  <v-progress-circular indeterminate color="primary" />
                </v-row>

                <template v-else>
                  <div class="d-flex align-center justify-space-between mb-3">
                    <label class="font-weight-medium">
                      {{ t('flights.pilgrimAssignment.assignedCount', { count: assignedTotal }) }}
                    </label>
                    <v-text-field
                      v-model="searchQuery"
                      :placeholder="t('flights.pilgrimAssignment.searchAssigned')"
                      density="compact"
                      hide-details
                      rounded="lg"
                      clearable
                      style="max-width: 250px"
                    />
                  </div>

                  <v-table v-if="assignedPilgrims.length > 0">
                    <thead>
                      <tr>
                        <th class="text-center">#</th>
                        <th class="text-right">{{ t('pilgrims.headers.name') }}</th>
                        <th class="text-right">{{ t('pilgrims.headers.identity') }}</th>
                        <th class="text-right">{{ t('pilgrims.headers.nationality') }}</th>
                        <th class="text-center" />
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(p, i) in assignedPilgrims" :key="p.pilgrimId">
                        <td class="text-center text-medium-emphasis">
                          {{ (i as number) + 1 + (assignedPage - 1) * assignedPerPage }}
                        </td>
                        <td>{{ p.name }}</td>
                        <td class="text-body-2 text-medium-emphasis">{{ p.identity }}</td>
                        <td>{{ p.nationality }}</td>
                        <td class="text-center">
                          <v-btn
                            size="x-small"
                            color="error"
                            variant="text"
                            :loading="unassigningId === p.pilgrimId"
                            :disabled="isAssigning"
                            @click="handleUnassign(p.pilgrimId)"
                          >
                            <template #prepend>
                              <Trash :size="14" />
                            </template>
                            {{ t('flights.pilgrimAssignment.unassignButton') }}
                          </v-btn>
                        </td>
                      </tr>
                    </tbody>
                  </v-table>

                  <p v-else class="text-center text-medium-emphasis py-6">
                    {{ searchQuery ? t('common.noResults') : t('flights.pilgrimAssignment.noAssigned') }}
                  </p>
                </template>
              </div>

              <div v-if="assignedTotalPages > 1" class="flex-shrink-0 px-6 pb-2">
                <Pagination
                  v-model:page="assignedPage"
                  v-model:per-page="assignedPerPage"
                  :total-items="assignedTotal"
                />
              </div>
            </div>
          </v-tabs-window-item>
        </v-tabs-window>
      </v-card-text>

      <v-card-actions class="bg-primary_2 rounded-lg py-3 justify-center flex-shrink-0">
        <v-btn class="rounded-xl px-8" variant="outlined" @click="dialog = false">
          {{ t('common.close') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { logger } from '@/@core/utils/logger'
import { flightsService } from '@/services/flights.service'
import { pilgrimsService } from '@/services/pilgrims.service'
import { useAuthStore } from '@/stores/auth.store'
import { useAppStore } from '@/stores/app.store'
import Pagination from '@/@core/components/Pagination.vue'
import Trash from '@/components/icons/trash.vue'
import type { Flight, FlightPilgrimAssignment } from '@/types'

const props = defineProps<{ flightId: number }>()
const dialog = defineModel<boolean>()

const { t } = useI18n()
const authStore = useAuthStore()
const appStore = useAppStore()

const tab = ref('assign')
const flight = ref<Flight | null>(null)

// ── Assign tab state ──
const availablePilgrims = ref<{ id: number; name: string; identity: string }[]>([])
const availablePage = ref(1)
const availablePerPage = ref(10)
const availableTotal = ref(0)
const availableTotalPages = computed(() => Math.ceil(availableTotal.value / availablePerPage.value) || 1)
const availableLoading = ref(false)
const searchAvailableQuery = ref('')
const selectedPilgrimIds = ref<number[]>([])

// ── Assigned tab state ──
const assignedPilgrims = ref<FlightPilgrimAssignment[]>([])
const assignedPage = ref(1)
const assignedPerPage = ref(10)
const assignedTotal = ref(0)
const assignedTotalPages = computed(() => Math.ceil(assignedTotal.value / assignedPerPage.value) || 1)
const assignedLoading = ref(false)
const searchQuery = ref('')

// ── Shared state ──
const isAssigning = ref(false)
const unassigningId = ref<number | null>(null)

const isCapacityFull = computed(() => {
  if (!flight.value?.capacity || flight.value.capacity === 0) return false
  return assignedTotal.value >= flight.value.capacity
})

const togglePilgrim = (id: number) => {
  const idx = selectedPilgrimIds.value.indexOf(id)
  if (idx === -1) {
    selectedPilgrimIds.value.push(id)
  } else {
    selectedPilgrimIds.value.splice(idx, 1)
  }
}

// ── Fetch flight (first load) ──
const fetchFlight = async () => {
  try {
    const flightRes = await flightsService.getById(props.flightId)
    flight.value = flightRes.data
  } catch (error) {
    logger.error('Error loading flight:', error)
  }
}

// ── Fetch available pilgrims (assign tab) ──
const fetchAvailablePilgrims = async () => {
  availableLoading.value = true
  try {
    const pilgrimCompanyId = authStore.isCompany ? authStore.company?.id : flight.value?.companyId
    if (!pilgrimCompanyId) return

    const res = await pilgrimsService.getAll({
      companyId: pilgrimCompanyId,
      excludeFlightId: props.flightId,
      page: availablePage.value,
      perPage: availablePerPage.value,
      search: searchAvailableQuery.value || undefined,
    })
    availablePilgrims.value = (res.data || []).map((p) => ({ id: p.id, name: p.name, identity: p.identity }))
    availableTotal.value = res.pagination?.totalObjects ?? 0
  } catch (error) {
    logger.error('Error loading available pilgrims:', error)
    appStore.showSnackbar({ message: t('common.error'), color: 'error' })
  } finally {
    availableLoading.value = false
  }
}

// ── Fetch assigned pilgrims (assigned tab) ──
const fetchAssignedPilgrims = async () => {
  assignedLoading.value = true
  try {
    const res = await flightsService.getAssignedPilgrims(props.flightId, {
      page: assignedPage.value,
      perPage: assignedPerPage.value,
    })
    assignedPilgrims.value = res.data?.items || []
    assignedTotal.value = res.data?.pagination?.totalObjects ?? 0
  } catch (error) {
    logger.error('Error loading assigned pilgrims:', error)
    appStore.showSnackbar({ message: t('common.error'), color: 'error' })
  } finally {
    assignedLoading.value = false
  }
}

// ── Handlers ──
const handleAssign = async () => {
  if (selectedPilgrimIds.value.length === 0) return
  isAssigning.value = true
  try {
    for (const pilgrimId of selectedPilgrimIds.value) {
      await flightsService.assignPilgrim(props.flightId, pilgrimId)
    }
    appStore.showSnackbar({ message: t('flights.pilgrimAssignment.messages.assignSuccess'), color: 'success' })
    selectedPilgrimIds.value = []
    availablePage.value = 1
    await Promise.all([fetchAvailablePilgrims(), fetchAssignedPilgrims()])
  } catch (error) {
    const msg = (error as { response?: { data?: { message?: string } } }).response?.data?.message || t('common.error')
    appStore.showSnackbar({ message: msg, color: 'error' })
  } finally {
    isAssigning.value = false
  }
}

const handleUnassign = async (pilgrimId: number) => {
  unassigningId.value = pilgrimId
  try {
    await flightsService.unassignPilgrim(props.flightId, pilgrimId)
    appStore.showSnackbar({ message: t('flights.pilgrimAssignment.messages.unassignSuccess'), color: 'success' })
    await Promise.all([fetchAvailablePilgrims(), fetchAssignedPilgrims()])
  } catch (error) {
    const msg = (error as { response?: { data?: { message?: string } } }).response?.data?.message || t('common.error')
    appStore.showSnackbar({ message: msg, color: 'error' })
  } finally {
    unassigningId.value = null
  }
}

// ── Watchers ──
watch(
  dialog,
  (open: boolean) => {
    if (open) {
      tab.value = 'assign'
      fetchFlight()
      fetchAvailablePilgrims()
      fetchAssignedPilgrims()
    } else {
      selectedPilgrimIds.value = []
      searchQuery.value = ''
      searchAvailableQuery.value = ''
      availablePage.value = 1
      assignedPage.value = 1
      availablePilgrims.value = []
      assignedPilgrims.value = []
      flight.value = null
    }
  },
  { immediate: true },
)

watch(availablePage, () => {
  fetchAvailablePilgrims()
})
watch(availablePerPage, () => {
  availablePage.value = 1
  fetchAvailablePilgrims()
})
watch(searchAvailableQuery, () => {
  availablePage.value = 1
  fetchAvailablePilgrims()
})

watch(assignedPage, () => {
  fetchAssignedPilgrims()
})
watch(assignedPerPage, () => {
  assignedPage.value = 1
  fetchAssignedPilgrims()
})
watch(searchQuery, () => {
  assignedPage.value = 1
  fetchAssignedPilgrims()
})
</script>

<style scoped lang="scss">
.pilgrim-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}

.pilgrim-card {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  cursor: pointer;
  transition:
    border-color 0.15s,
    background-color 0.15s;
}

.pilgrim-card:hover {
  border-color: #a8aaae;
  background-color: #f5f5f5;
}

.pilgrim-card-selected {
  border-color: #9155fd;
  background-color: #f3ecff;
}

.pilgrim-check-icon {
  flex-shrink: 0;
}

.pilgrim-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.pilgrim-name {
  font-weight: 500;
  font-size: 0.9rem;
  line-height: 1.3;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.pilgrim-identity {
  font-size: 0.8rem;
  color: #667178;
  line-height: 1.3;
}

.v-tabs {
  height: 58px;
}

:deep(.v-tabs button .v-btn__overlay) {
  // background-color: rgb(var(--v-theme-on-primary));
}
:deep(.v-tabs button.v-tab--selected .v-btn__overlay) {
  // background-color: rgb(var(--v-theme-primary));
}
</style>
