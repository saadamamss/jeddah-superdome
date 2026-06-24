<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useConfirmDialog } from '@/composables/useConfirmDialog'

const { t } = useI18n()

const props = defineProps({
  modelValue: Boolean,
  currentRoleId: {
    type: Number,
    default: null,
  },
  isDeleting: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['close', 'update:modelValue', 'delete'])

const { isProcessing } = useConfirmDialog({
  onConfirm: async (id: number) => {
    emit('delete', id)
  },
})

const closeDialog = () => {
  emit('close')
  emit('update:modelValue', false)
}
</script>

<template>
  <v-dialog
    :model-value="modelValue"
    persistent
    width="625"
    @update:model-value="(val: boolean) => emit('update:modelValue', val)"
  >
    <v-card class="py-4 px-2" rounded="lg">
      <v-card-title class="px-4 border-b pb-4 font-weight-bold">
        <div class="w-100 d-flex align-center justify-space-between">
          <h2 class="mb-0 text-h6 font-weight-bold">{{ t('roles.delete.confirmTitle') }}</h2>
          <v-btn color="black" :disabled="isProcessing" icon variant="text" @click="closeDialog">
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="mb-6 border-b pb-4">
        <v-row>
          <v-col class="py-8 text-center" cols="12">
            <div class="d-flex justify-center mb-6">
              <svg fill="none" height="146" viewBox="0 0 140 146" width="140" xmlns="http://www.w3.org/2000/svg">
                <path
                  d="M130 0.5H10C4.47715 0.5 0 4.97715 0 10.5V130.5C0 136.023 4.47715 140.5 10 140.5H130C135.523 140.5 140 136.023 140 130.5V10.5C140 4.97715 135.523 0.5 130 0.5Z"
                  fill="#FFF0F0"
                />
                <path
                  d="M115 145.5H25V48.5C29.242 48.4952 33.3089 46.808 36.3084 43.8085C39.308 40.8089 40.9952 36.742 41 32.5H99C98.9955 34.6014 99.4078 36.6828 100.213 38.6238C101.018 40.5648 102.2 42.3268 103.691 43.808C105.172 45.2991 106.934 46.4816 108.875 47.287C110.817 48.0924 112.898 48.5047 115 48.5V145.5Z"
                  fill="white"
                />
                <path
                  d="M70 97.5C83.2548 97.5 94 86.7548 94 73.5C94 60.2452 83.2548 49.5 70 49.5C56.7452 49.5 46 60.2452 46 73.5C46 86.7548 56.7452 97.5 70 97.5Z"
                  fill="#C05E5E"
                />
                <path
                  clip-rule="evenodd"
                  d="M70.0883 59.667H70.0029C69.3292 59.667 68.7577 59.667 68.2834 59.7101C67.7802 59.7559 67.3114 59.8552 66.8581 60.0962C66.6792 60.1913 66.5085 60.3011 66.3477 60.4245C65.9404 60.737 65.6556 61.1224 65.4052 61.5613C65.1692 61.9749 64.9322 62.4949 64.6528 63.108L64.0941 64.3337H60.0018C60.0012 64.3337 60.0007 64.3337 60.0002 64.3337H58C57.4477 64.3337 57 64.7814 57 65.3337C57 65.8859 57.4477 66.3337 58 66.3337H59.0586L59.8111 78.7991C59.9117 80.4653 59.9915 81.7875 60.1575 82.8437C60.3278 83.9268 60.6016 84.8289 61.1508 85.618C61.6532 86.34 62.3005 86.9493 63.0514 87.4073C63.8722 87.908 64.7891 88.1269 65.8805 88.2316C66.9448 88.3337 68.2693 88.3337 69.9385 88.3337H70.042C71.709 88.3337 73.032 88.3337 74.0949 88.2318C75.1851 88.1273 76.101 87.9087 76.9212 87.4091C77.6717 86.9519 78.3187 86.3436 78.8213 85.6228C79.3707 84.835 79.6453 83.9343 79.8169 82.8526C79.9842 81.798 80.0658 80.4776 80.1686 78.8138L80.9401 66.3337H82C82.5523 66.3337 83 65.8859 83 65.3337C83 64.7814 82.5523 64.3337 82 64.3337H80.0009C79.9997 64.3337 79.9985 64.3337 79.9973 64.3337H76.0342L75.3788 62.9818C75.0922 62.3905 74.8489 61.8886 74.6094 61.4895C74.3551 61.0659 74.0693 60.6947 73.6668 60.3945C73.5078 60.2758 73.3393 60.1703 73.1632 60.0789C72.7175 59.8477 72.2588 59.7524 71.7667 59.7084C71.3031 59.667 70.7453 59.667 70.0883 59.667ZM78.9363 66.3337H75.4297C75.4154 66.334 75.4012 66.334 75.387 66.3337H64.7587C64.744 66.334 64.7293 66.334 64.7146 66.3337H61.0622L61.8044 78.627C61.9088 80.3564 61.9839 81.5827 62.1333 82.5331C62.2802 83.4674 62.4859 84.0351 62.7924 84.4756C63.1362 84.9696 63.579 85.3865 64.0928 85.6999C64.551 85.9793 65.1301 86.1504 66.0715 86.2408C67.0291 86.3326 68.2578 86.3337 69.9904 86.3337C71.7206 86.3337 72.9476 86.3326 73.9041 86.2409C74.8443 86.1508 75.4228 85.98 75.8807 85.701C76.3941 85.3883 76.8369 84.972 77.1808 84.4789C77.4874 84.0391 77.6936 83.4722 77.8416 82.5393C77.9921 81.5904 78.0689 80.3658 78.1756 78.6388L78.9363 66.3337ZM73.8116 64.3337H66.3757L65.7413 78.6988C65.6599 80.3758 65.6004 81.6598 65.557 82.5697C65.5149 83.4557 65.5104 83.866 65.5785 84.1832C65.6442 84.4919 65.7792 84.7748 65.9811 85.0103C66.1921 85.2555 66.4843 85.4329 67.0223 85.516C67.5785 85.6018 68.3567 85.6004 69.865 85.6004H70.1174C71.6236 85.6004 72.4012 85.6018 72.957 85.516C73.4942 85.4329 73.7861 85.2555 73.997 85.0103C74.1988 84.7748 74.3337 84.4919 74.3993 84.1832C74.4674 83.866 74.463 83.4557 74.4209 82.5697C74.3775 81.6598 74.318 80.3758 74.2366 78.6988L73.8116 64.3337ZM63.2714 66.3337H58C57.4477 66.3337 57 66.7814 57 67.3337C57 67.8859 57.4477 68.3337 58 68.3337H63.6849L63.2714 66.3337ZM76.5966 68.3337H82C82.5523 68.3337 83 67.8859 83 67.3337C83 66.7814 82.5523 66.3337 82 66.3337H76.3153L76.5966 68.3337Z"
                  fill="white"
                  fill-rule="evenodd"
                />
                <path
                  d="M83 103.5H57C55.3431 103.5 54 104.843 54 106.5C54 108.157 55.3431 109.5 57 109.5H83C84.6569 109.5 86 108.157 86 106.5C86 104.843 84.6569 103.5 83 103.5Z"
                  fill="#FFF0F0"
                />
                <path
                  d="M92 115.5H48C46.3431 115.5 45 116.843 45 118.5C45 120.157 46.3431 121.5 48 121.5H92C93.6569 121.5 95 120.157 95 118.5C95 116.843 93.6569 115.5 92 115.5Z"
                  fill="#FFF0F0"
                />
              </svg>
            </div>
            <h2 class="text-xl mb-4">{{ t('roles.delete.title') }}</h2>
            <p class="mx-auto" style="max-width: 500px">
              {{ t('roles.delete.subtitle') }}
            </p>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions>
        <v-row>
          <v-col cols="6">
            <v-btn
              block
              color="error"
              :loading="isProcessing"
              :disabled="isProcessing"
              rounded="pill"
              size="large"
              style="background-color: #faeeeb"
              variant="text"
              @click="emit('delete', props.currentRoleId)"
            >
              <span class="d-inline-block px-4">{{ t('roles.delete.action') }}</span>
            </v-btn>
          </v-col>
          <v-col cols="6">
            <v-btn
              block
              color="primary"
              :disabled="isProcessing"
              rounded="pill"
              size="large"
              variant="elevated"
              @click="closeDialog"
            >
              <span class="d-inline-block px-4">{{ t('roles.delete.cancel') }}</span>
            </v-btn>
          </v-col>
        </v-row>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
