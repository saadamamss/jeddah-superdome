<template>
  <v-card-text class="py-2" outlined>
    <!-- Title -->
    <v-card-title class="text-h6 justify-center">
      {{ title }}
    </v-card-title>
    <!-- Uploaded File Info -->
    <v-alert v-if="uploadedFile" type="" class="mb-6 border rounded-lg">
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex gap-2 align-center">
          <img src="/imgs/icons/csv-file-icon.svg" alt="" />
          <div>
            <span class="text-lg">{{ uploadedFile.name }}</span>
            <p class="text-error mb-0 text-sm" v-if="uploadError?.message">
              {{ uploadError.message }}
            </p>
            <p class="mb-0 text-secondary text-sm" v-else>
              {{ formatFileSize(uploadedFile.size) }}
            </p>
          </div>
        </div>
        <div class="d-flex gap-2">
            <v-btn small text @click="removeFile" color="#fff" size="small">
              <v-icon size="large" color="primary" v-if="uploadError?.message"
                >mdi-replay</v-icon
              >
            </v-btn>
            <v-btn small text @click="removeFile" color="#fff" size="small">
              <v-icon size="large" color="error">mdi-trash</v-icon>
            </v-btn>
        </div>
      </div>
    </v-alert>
    <!-- Drag & Drop Zone -->
    <v-card
      v-else
      @click="$refs.fileInput.click()"
      @dragover.prevent="dragOver = true"
      @dragleave="dragOver = false"
      @drop.prevent="handleDrop"
      :class="['drop-zone', { dragover: dragOver }]"
      outlined
      class="text-center bg-primary_2 py-6 mb-6"
    >
      <img src="/imgs/icons/upload.svg" alt="" />
      <p class="mt-4 mb-2">اسحب وأفلِت أو اختر الملف الذي تريد تحميله</p>
      <p class="text-secondary text-xs">
        يدعم الامتدادات .xlsx, .csv والحد الأقصى للحجم 5 ميجا بايت.
      </p>
      <v-btn color="#fff" class="mt-2 rounded-xl">استعراض الملفات</v-btn>
      <input
        type="file"
        ref="fileInput"
        @change="handleFileSelect"
        accept=".csv"
        hidden
      />
    </v-card>
    <v-card-text class="rounded-xl py-2" style="background-color: #f8f5ef">
      <div class="d-flex align-center justify-space-between">
        <p class="text-sm mb-0">
          يرجى التأكد من تنسيق الأعمدة حسب النموذج المطلوب
        </p>
        <v-btn
          color="#fff"
          size="small"
          class="text-black bg-white px-5 text-sm"
          variant="text"
          rounded="xl"
          >{{btnText}}
          </v-btn
        >
      </div>
    </v-card-text>
  </v-card-text>
</template>

<script>
export default {
  props: ["title", "uploadError" , 'btnText'],
  data() {
    return {
      dragOver: false,
      uploadedFile: null,
      previewData: [],
      previewHeaders: [],
    };
  },
  methods: {
    formatFileSize(bytes) {
      if (bytes === 0) return "0 Bytes";

      const k = 1024;
      const sizes = ["Bytes", "KB", "MB", "GB", "TB"];
      const i = Math.floor(Math.log(bytes) / Math.log(k));

      return sizes[i] + " " + parseFloat((bytes / Math.pow(k, i)).toFixed(2));
    },
    handleFileSelect(event) {
      this.uploadedFile = event.target.files[0];
      this.dragOver = false;
      if (this.uploadedFile) {
        this.$emit("file-selected", this.uploadedFile);
      }
    },
    handleDrop(event) {
      const files = event.dataTransfer.files;
      if (files.length && files[0].name.endsWith(".csv")) {
        this.uploadedFile = files[0];
      }
      this.dragOver = false;
    },
    removeFile() {
      this.uploadedFile = null;
      this.previewData = [];
      this.$refs.fileInput.value = "";
    },
  },
};
</script>

<style scoped>
.drop-zone {
  border: 2px dashed #ddd;
  cursor: pointer;
  transition: background 0.3s;
}
.dragover {
  background: rgba(25, 118, 210, 0.1);
}
</style>
