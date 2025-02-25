<script setup>
const emit = defineEmits(["close"]);

const formState = reactive({
  name: null,
  desc: null,
  permissions: [],
});

const closeDialog = () => {
  emit("close");
};
</script>

<template>
  <v-dialog width="625">
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 border-b pb-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">إضافة دور جديد</h2>
          <v-btn icon variant="text" color="black" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6 border-b pb-4">
        <v-row>
          <v-col cols="12">
            <label class="d-inline-block mb-1 text-black">اسم الدور</label>
            <v-text-field
              v-model="formState.name"
              variant="text"
              class="mb-1 rounded-lg"
              density="comfortable"
              style="background-color: #f9f9f9"
              placeholder="اسم الدور"
            ></v-text-field>
          </v-col>
          <v-col cols="12">
            <label class="d-flex ga-1 align-center mb-1 text-black">
              <span>وصف الدور</span>
              <small style="color: #667178">(اختياري)</small>
            </label>
            <v-textarea
              v-model="formState.desc"
              variant="text"
              class="mb-1 rounded-lg"
              density="comfortable"
              style="background-color: #f9f9f9"
              placeholder="وصف الدور..."
            ></v-textarea>
          </v-col>
          <v-col cols="12">
            <label class="d-flex ga-1 align-center mb-1 text-black">
              صلاحيات الدور
            </label>
            <v-select
              v-model="formState.permissions"
              :items="['مراقب ميداني', 'مشرف ميداني']"
              min-width="100%"
              bg-color="#F8F9F9"
              rounded="lg"
              variant="solo"
              multiple
              placeholder="اختر الصلاحيات"
              class="v-select-no-shadow"
            />
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions>
        <v-btn
          variant="text"
          rounded="lg"
          color="black"
          style="background-color: #f8f9f9"
          @click="closeDialog"
        >
          <span class="d-inline-block px-4">الغاء</span>
        </v-btn>
        <v-spacer></v-spacer>
        <v-btn color="primary" variant="elevated" rounded="lg">
          <span class="d-inline-block px-4">إضافة </span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
