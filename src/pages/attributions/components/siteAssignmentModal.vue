<script setup>
import { ref, watch, computed } from "vue";
import CustomVAutocomplete from "@/@core/components/global/CustomVAutocomplete.vue";
import SearchableAutocomplete from "@/@core/components/global/SearchableAutocomplete.vue";
import { validationRules } from "@/@core/utils/validators";

const props = defineProps({
  modelValue: {
    type: Boolean,
    required: true,
  },
  inspectorId: {
    type: [String, Number],
    default: null,
  },
  showInspectorSelect: {
    type: Boolean,
    default: false,
  },
  inspectors: {
    type: Array,
    default: () => [],
  },
  locations: {
    type: Array,
    default: () => [],
  },
  companies: {
    type: Array,
    default: () => [],
  },
  loadingLocations: {
    type: Boolean,
    default: false,
  },
  loadingCompanies: {
    type: Boolean,
    default: false,
  },
  loadingInspectors: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["update:modelValue", "save"]);

const formData = ref({
  location_id: null,
  company_id: null,
  inspector_ids: [],
});

const selectedLocation = ref(null);
const selectedCompany = ref(null);
const selectedInspectors = ref([]);
const searchLocation = ref("");
const searchCompany = ref("");
const searchInspector = ref("");

const locationItems = computed(() => {
  return props.locations.map((location) => ({
    label: `${location?.name || "غير محدد"} ${
      location?.location_type?.title ? `(${location.location_type.title})` : ""
    }`,
    value: location?.id,
    sublabel: `رقم الرخصة: ${
      location?.license_no || location?.camp_number || "-"
    } | المندوب: ${location?.delegate_name || "-"} | جوال: ${
      location?.delegate_phone || "-"
    }`,
    raw: location,
  }));
});

const companyItems = computed(() => {
  return props.companies.map((company) => ({
    label: company.name,
    value: company.id,
    sublabel: company.parent_company_name
      ? `${company.parent_company_name} - ${
          company.pligrims_nationality || "-"
        }`
      : "-",
    raw: company,
  }));
});

const inspectorItems = computed(() => {
  return props.inspectors.map((inspector) => ({
    label: inspector.name || `${inspector.firstname} ${inspector.lastname}`,
    value: inspector.id,
    sublabel: inspector.sa_id ? `رقم الهوية: ${inspector.sa_id}` : "",
    raw: inspector,
  }));
});

const filteredLocations = computed(() => {
  if (!searchLocation.value) return locationItems.value;
  const search = searchLocation.value.toLowerCase();
  return locationItems.value.filter(
    (item) =>
      item.label.toLowerCase().includes(search) ||
      item.sublabel.toLowerCase().includes(search)
  );
});

const filteredCompanies = computed(() => {
  if (!searchCompany.value) return companyItems.value;
  const search = searchCompany.value.toLowerCase();
  return companyItems.value.filter(
    (item) =>
      item.label.toLowerCase().includes(search) ||
      item.sublabel.toLowerCase().includes(search)
  );
});

const filteredInspectors = computed(() => {
  if (!searchInspector.value) return inspectorItems.value;
  const search = searchInspector.value.toLowerCase();
  return inspectorItems.value.filter(
    (item) =>
      item.label.toLowerCase().includes(search) ||
      (item.raw.sa_id && item.raw.sa_id.toLowerCase().includes(search))
  );
});

const resetForm = () => {
  formData.value = {
    location_id: null,
    company_id: null,
    inspector_ids: [],
  };
  selectedLocation.value = null;
  selectedCompany.value = null;
  selectedInspectors.value = [];
  searchLocation.value = "";
  searchCompany.value = "";
  searchInspector.value = "";
};

const handleSave = () => {
  const data = {
    ...formData.value,
    location_details: selectedLocation.value?.raw,
    company_details: selectedCompany.value?.raw,
  };

  // If not showing inspector select but have inspectorId, use it
  if (!props.showInspectorSelect && props.inspectorId) {
    data.inspector_ids = [props.inspectorId];
  }
  // If showing inspector select, use selected inspectors
  else if (props.showInspectorSelect) {
    data.inspector_ids = selectedInspectors.value.map((item) => item.value);
    data.inspector_details = selectedInspectors.value.map(
      (inspector) => inspector.raw
    );
  }

  emit("save", data);
  resetForm();
};

const handleClose = () => {
  emit("update:modelValue", false);
  resetForm();
};

watch(
  () => props.modelValue,
  (newVal) => {
    if (!newVal) {
      resetForm();
    }
  }
);

// Watch for inspector selection changes
watch(
  () => selectedInspectors.value,
  (newVal) => {
    formData.value.inspector_ids = newVal.map((item) => item.value);
  }
);
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="handleClose"
    max-width="600px"
    persistent
  >
    <v-card class="rounded-lg">
      <v-card-title class="text-h5 pa-4"> إسناد موقع جديد </v-card-title>

      <v-card-text class="pa-4">
        <v-form @submit.prevent="handleSave">
          <v-row>
            <!-- Inspector Selection - Only show if showInspectorSelect is true -->
            <v-col cols="12" v-if="showInspectorSelect">
              <!-- v-model="formData.inspector_ids" -->
              <CustomVAutocomplete
                v-model="formData.inspector_ids"
                v-model:search="searchInspector"
                :items="filteredInspectors"
                label="اختر المراقبين"
                :loading="loadingInspectors"
                placeholder="ابحث عن مراقبين..."
                no-data-text="لا توجد نتائج"
                multiple
                chips
                closable-chips
              >
                <template #item="{ item }">
                  <v-list-item>
                    <v-list-item-title>{{ item.raw.label }}</v-list-item-title>
                    <v-list-item-subtitle>{{
                      item.raw.sublabel
                    }}</v-list-item-subtitle>
                  </v-list-item>
                </template>
              </CustomVAutocomplete>
            </v-col>

            <v-col cols="12">
              <!-- v-model="selectedLocation"
              v-model:search="searchLocation"
              :items="filteredLocations"
              label="اختر الموقع"
              :loading="loadingLocations"
              placeholder="ابحث عن موقع..."
              no-data-text="لا توجد نتائج"
              @update:model-value="formData.location_id = $event?.value" -->
              <SearchableAutocomplete
                v-model="formData.location_id"
                search-url="/locations"
                responseDataKey="locations"
                label="اختر الموقع"
                item-title="name"
                item-value="id"
                clearable
                :return-object="false"
                :rules="[validationRules.required]"
              >
                <template #item="{ item }">
                  <v-list-item>
                    <v-list-item-title>{{ item.raw.label }}</v-list-item-title>
                    <v-list-item-subtitle>{{
                      item.raw.sublabel
                    }}</v-list-item-subtitle>
                  </v-list-item>
                </template>
              </SearchableAutocomplete>
            </v-col>

            <v-col cols="12">
              <!-- v-model="selectedCompany"
              v-model:search="searchCompany"
              :items="filteredCompanies"
              label="اختر الشركة"
              :loading="loadingCompanies"
              placeholder="ابحث عن شركة..."
              no-data-text="لا توجد نتائج"
              @update:model-value="formData.company_id = $event?.value" -->
              <SearchableAutocomplete
                v-model="formData.company_id"
                search-url="/companies"
                item-title="name"
                item-value="id"
                label="اختر الشركة"
                clearable
                :return-object="false"
                :rules="[validationRules.required]"
              >
                <!-- :item-title-concat="['firstname', 'lastname']" -->
                <template #item="{ item }">
                  <v-list-item>
                    <v-list-item-title>{{ item.raw.label }}</v-list-item-title>
                    <v-list-item-subtitle>{{
                      item.raw.sublabel
                    }}</v-list-item-subtitle>
                  </v-list-item>
                </template>
              </SearchableAutocomplete>
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions class="pa-4">
        <v-spacer></v-spacer>
        <v-btn color="default" variant="text" @click="handleClose">
          إلغاء
        </v-btn>
        <v-btn color="primary" @click="handleSave"> حفظ </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<style lang="scss" scoped>
.v-card-title {
  border-bottom: 1px solid rgba(0, 0, 0, 0.12);
}

:deep(.v-field__input) {
  padding-top: 5px;
  min-height: 44px;
}

:deep(.v-list-item) {
  min-height: unset;
  padding: 8px 16px;

  .v-list-item-title {
    font-size: 0.875rem;
    font-weight: 500;
    margin-bottom: 2px;
  }

  .v-list-item-subtitle {
    font-size: 0.75rem;
    opacity: 0.7;
  }
}

.text-error {
  color: rgb(var(--v-theme-error)) !important;
}
</style>
