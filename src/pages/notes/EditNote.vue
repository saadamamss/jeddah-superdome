<template>
  <v-container fluid class="pa-0">
    <!-- Breadcrumbs -->
    <v-breadcrumbs
      :items="breadcrumbItems"
      class="pa-0 mb-4 custom-breadcrumbs"
      divider=""
    >
      <template v-slot:prepend>
        <svg
          width="20"
          height="20"
          viewBox="0 0 20 20"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M5.62516 16.0419H14.3752C15.2956 16.0419 16.0418 15.2957 16.0418 14.3752V8.12523L10.0002 3.95856L3.9585 8.12523V14.3752C3.9585 15.2957 4.70469 16.0419 5.62516 16.0419Z"
            stroke="#25343E"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
          <path
            d="M8.12465 13.1244C8.12465 12.2039 8.87085 11.4577 9.79132 11.4577H10.208C11.1285 11.4577 11.8747 12.2039 11.8747 13.1244V16.0411H8.12465V13.1244Z"
            stroke="#25343E"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </template>
      <template v-slot:divider>
        <v-icon size="18" class="text-grey-darken-1">mdi-chevron-left</v-icon>
      </template>
      <template v-slot:title="{ item }">
        <router-link
          v-if="!item.disabled"
          :to="item.href"
          class="text-decoration-none text-grey-darken-1 text-13"
        >
          {{ item.title }}
        </router-link>
        <span v-else class="text-grey-darken-1 text-13">{{ item.title }}</span>
      </template>
    </v-breadcrumbs>

    <v-card elevation="0" class="rounded-lg">
      <v-divider></v-divider>

      <v-card-text class="px-6 py-4">
        <!-- Note Details -->

        <div class="mb-6">
          <div class="mb-2 d-flex align-center justify-space-between">
            <div class="text-h5 font-weight-bold">نوع الملاحظة</div>
            <!-- Note Type Chips -->
            <div class="d-flex gap-2">
              <button
                class="font-weight-medium min-width-100px rounded-pill px-12 py-2 font-weight-bold"
                :style="{
                  backgroundColor: 'rgba(237, 244, 238, 1)',
                  color: 'rgba(78, 145, 88, 1)',
                }"
              >
                قبول
              </button>
              <button
                class="font-weight-medium min-width-100px rounded-pill px-12 py-2 font-weight-bold"
                :style="{
                  backgroundColor: 'rgba(250, 238, 235, 1)',
                  color: 'rgba(198, 40, 0, 1)',
                }"
              >
                رفض
              </button>
            </div>
          </div>
          <div class="text-sm mt-4 rounded-pill px-4 py-2">
            12 يناير 2024 — الساعة 10:45 صباحاً
          </div>
        </div>

        <!-- Note Description -->
        <div class="mb-6 border-t pt-4">
          <div class="text-h6 font-weight-bold mb-2">وصف الملاحظة</div>
          <v-card elevation="0" class="bg-section rounded-lg pa-4">
            <div class="text-body-1">{{ description }}</div>
          </v-card>
        </div>

        <!-- Note Details -->
        <div class="mb-6">
          <div class="text-h6 font-weight-bold mb-2">تفاصيل الملاحظة</div>
          <v-card elevation="0" class="bg-section rounded-lg pa-4">
            <div class="d-flex align-center gap-4 justify-space-between">
              <div class="d-flex align-center gap-4 mb-4">
                <svg
                  width="23"
                  height="24"
                  viewBox="0 0 23 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M20.6854 12.714V12.2385C20.6854 10.6372 20.6854 9.35368 20.6129 8.30957H3.16526C3.09277 9.35368 3.09277 10.6372 3.09277 12.2385V12.714C3.09277 16.7485 3.09277 18.7658 4.25214 20.0192C5.41151 21.2725 7.27749 21.2725 11.0094 21.2725H12.7687C16.5007 21.2725 18.3666 21.2725 19.526 20.0192C20.6854 18.7658 20.6854 16.7485 20.6854 12.714Z"
                    fill="#B69265"
                  />
                  <path
                    d="M7.25955 12.9392C7.25955 12.4278 7.6741 12.0133 8.18547 12.0133H8.19378C8.70516 12.0133 9.11971 12.4278 9.11971 12.9392C9.11971 13.4506 8.70516 13.8651 8.19378 13.8651H8.18547C7.6741 13.8651 7.25955 13.4506 7.25955 12.9392Z"
                    fill="#B69265"
                  />
                  <path
                    d="M10.9591 12.9392C10.9591 12.4278 11.3736 12.0133 11.885 12.0133H11.8933C12.4047 12.0133 12.8193 12.4278 12.8193 12.9392C12.8193 13.4506 12.4047 13.8651 11.8933 13.8651H11.885C11.3736 13.8651 10.9591 13.4506 10.9591 12.9392Z"
                    fill="#B69265"
                  />
                  <path
                    d="M14.6586 12.9392C14.6586 12.4278 15.0732 12.0133 15.5846 12.0133H15.5929C16.1043 12.0133 16.5188 12.4278 16.5188 12.9392C16.5188 13.4506 16.1043 13.8651 15.5929 13.8651H15.5846C15.0732 13.8651 14.6586 13.4506 14.6586 12.9392Z"
                    fill="#B69265"
                  />
                  <path
                    d="M7.25955 16.6429C7.25955 16.1315 7.6741 15.717 8.18547 15.717H8.19378C8.70516 15.717 9.11971 16.1315 9.11971 16.6429C9.11971 17.1543 8.70516 17.5688 8.19378 17.5688H8.18547C7.6741 17.5688 7.25955 17.1543 7.25955 16.6429Z"
                    fill="#B69265"
                  />
                  <path
                    d="M10.9591 16.6429C10.9591 16.1315 11.3736 15.717 11.885 15.717H11.8933C12.4047 15.717 12.8193 16.1315 12.8193 16.6429C12.8193 17.1543 12.4047 17.5688 11.8933 17.5688H11.885C11.3736 17.5688 10.9591 17.1543 10.9591 16.6429Z"
                    fill="#B69265"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M6.33362 2.05957C6.71715 2.05957 7.02807 2.37048 7.02807 2.75401V3.09281C8.07998 2.98547 9.37439 2.98548 10.9533 2.9855H12.8251C14.404 2.98548 15.6984 2.98547 16.7503 3.09281V2.75401C16.7503 2.37048 17.0612 2.05957 17.4447 2.05957C17.8283 2.05957 18.1392 2.37048 18.1392 2.75401V3.34178C18.8843 3.55298 19.5128 3.89624 20.0359 4.46176C20.7562 5.24045 21.0764 6.22431 21.2297 7.45726C21.3799 8.66541 21.3799 10.2137 21.3799 12.1893V12.7632C21.3799 14.7388 21.3799 16.2871 21.2297 17.4952C21.0764 18.7282 20.7562 19.712 20.0359 20.4907C19.3085 21.2771 18.3774 21.6337 17.2121 21.8031C16.0843 21.967 14.6433 21.967 12.8251 21.967H10.9533C9.13506 21.967 7.6941 21.967 6.56629 21.8031C5.401 21.6337 4.46987 21.2771 3.74246 20.4907C3.02217 19.712 2.702 18.7282 2.54867 17.4952C2.39842 16.2871 2.39843 14.7388 2.39844 12.7632V12.1892C2.39843 10.2137 2.39842 8.66541 2.54867 7.45726C2.702 6.22431 3.02217 5.24045 3.74246 4.46176C4.26557 3.89624 4.89403 3.55298 5.63918 3.34178V2.75401C5.63918 2.37048 5.95009 2.05957 6.33362 2.05957ZM6.33362 5.30031C6.01619 5.30031 5.7485 5.08733 5.66566 4.79646C5.28852 4.94652 5.00217 5.14527 4.76204 5.40487C4.32477 5.8776 4.06559 6.52564 3.92863 7.61513H19.8497C19.7128 6.52564 19.4536 5.8776 19.0163 5.40487C18.7762 5.14527 18.4898 4.94653 18.1127 4.79647C18.0299 5.08733 17.7622 5.30031 17.4447 5.30031C17.0612 5.30031 16.7503 4.9894 16.7503 4.60587V4.48974C15.7679 4.37583 14.4981 4.37439 12.7688 4.37439H11.0095C9.28026 4.37439 8.01048 4.37583 7.02807 4.48974V4.60587C7.02807 4.9894 6.71715 5.30031 6.33362 5.30031ZM19.955 9.00401C19.9905 9.89002 19.991 10.9492 19.991 12.2385V12.714C19.991 14.7494 19.9898 16.2114 19.8514 17.3238C19.7149 18.4215 19.4554 19.0729 19.0163 19.5476C18.5844 20.0146 18.0028 20.2847 17.0123 20.4286C15.9959 20.5764 14.656 20.5781 12.7688 20.5781H11.0095C9.1224 20.5781 7.78247 20.5764 6.76607 20.4286C5.77555 20.2847 5.19401 20.0146 4.76204 19.5476C4.32296 19.0729 4.06345 18.4215 3.92694 17.3238C3.78859 16.2114 3.78733 14.7494 3.78733 12.714V12.2385C3.78733 10.9492 3.78783 9.89002 3.82337 9.00401H19.955Z"
                    fill="#B69265"
                  />
                </svg>
                <div>
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">
                    تاريخ ووقت الملاحظة
                  </div>
                  <div class="text-body-1">
                    12 يناير 2024 — الساعة 10:45 صباحاً
                  </div>
                </div>
              </div>
              <div class="d-flex align-center gap-4 mb-4 justify-start w-50">
                <svg
                  width="24"
                  height="27"
                  viewBox="0 0 24 27"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    d="M6.57757 17.6066C5.1628 18.449 1.45336 20.1691 3.71266 22.3216C4.81631 23.373 6.04549 24.125 7.59087 24.125H16.4091C17.9545 24.125 19.1837 23.373 20.2873 22.3216C22.5466 20.1691 18.8372 18.449 17.4224 17.6066C14.1048 15.6311 9.89519 15.6311 6.57757 17.6066Z"
                    fill="#B69265"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M12 3.375C9.1005 3.375 6.75 5.7255 6.75 8.625C6.75 11.5245 9.1005 13.875 12 13.875C14.8995 13.875 17.25 11.5245 17.25 8.625C17.25 5.7255 14.8995 3.375 12 3.375ZM8.25 8.625C8.25 6.55393 9.92893 4.875 12 4.875C14.0711 4.875 15.75 6.55393 15.75 8.625C15.75 10.6961 14.0711 12.375 12 12.375C9.92893 12.375 8.25 10.6961 8.25 8.625Z"
                    fill="#B69265"
                  />
                  <path
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M18.2288 17.2042C18.0662 17.1124 17.9226 17.0314 17.8063 16.9622C14.2523 14.8459 9.74809 14.8459 6.19404 16.9622C6.07773 17.0314 5.93405 17.1125 5.77139 17.2043C5.05857 17.6064 3.98106 18.2143 3.2429 18.9368C2.78123 19.3887 2.34257 19.9842 2.26283 20.7138C2.17802 21.4896 2.51649 22.2177 3.1955 22.8646C4.36695 23.9806 5.77274 24.875 7.59104 24.875H16.4093C18.2276 24.875 19.6334 23.9806 20.8048 22.8646C21.4839 22.2177 21.8223 21.4896 21.7375 20.7138C21.6578 19.9842 21.2191 19.3887 20.7575 18.9368C20.0193 18.2143 18.9417 17.6063 18.2288 17.2042ZM6.96146 18.251C10.0426 16.4163 13.9577 16.4163 17.0389 18.251C17.2068 18.351 17.3907 18.4553 17.5834 18.5647C18.296 18.9689 19.1281 19.441 19.7082 20.0088C20.0683 20.3613 20.2218 20.6521 20.2464 20.8767C20.2659 21.0551 20.2208 21.3492 19.7702 21.7785C18.7343 22.7654 17.6818 23.375 16.4093 23.375H7.59104C6.3186 23.375 5.26602 22.7654 4.23017 21.7785C3.77954 21.3492 3.73445 21.0551 3.75395 20.8767C3.77851 20.6521 3.93202 20.3613 4.29214 20.0088C4.87223 19.441 5.70426 18.969 6.41686 18.5647C6.60953 18.4554 6.7936 18.351 6.96146 18.251Z"
                    fill="#B69265"
                  />
                </svg>
                <div>
                  <div class="text-subtitle-2 text-medium-emphasis mb-1">
                    مرسل الملاحظة
                  </div>
                  <div class="d-flex align-center gap-2">
                    <v-avatar color="primary" size="30">
                      <v-icon color="white" size="20">mdi-account</v-icon>
                    </v-avatar>
                    <span class="text-body-1">عبدالله القحطاني</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="d-flex align-center gap-4">
              <svg
                width="23"
                height="24"
                viewBox="0 0 23 24"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g opacity="0.4">
                  <path
                    d="M8.92318 17.1631C8.87902 17.0944 8.87902 17.0007 8.87902 16.8131L8.87902 2.65627C8.87902 2.34332 8.87902 2.18685 8.99496 2.10374L9.00442 2.09725C9.12369 2.019 9.2589 2.07116 9.52931 2.17549C9.96747 2.34455 10.4749 2.59688 11.0944 2.90489L13.6634 4.18229C13.8133 4.25685 13.8883 4.29412 13.93 4.36135C13.9716 4.42857 13.9716 4.51229 13.9716 4.67975V10.4731C13.9716 10.5796 13.9716 10.6329 13.9471 10.6757C13.9225 10.7185 13.8746 10.7465 13.7788 10.8025C12.6686 11.4515 11.7505 12.439 11.2177 13.6845C10.6416 15.0314 10.6245 16.2729 10.9188 17.3699C11.0358 17.8061 11.0943 18.0243 11.0017 18.1C10.9091 18.1758 10.7178 18.0807 10.3352 17.8904L10.0584 17.7528C9.70753 17.5783 9.44755 17.4505 9.24267 17.3572C9.05912 17.2736 8.96734 17.2317 8.92318 17.1631Z"
                    fill="#B69265"
                  />
                  <path
                    d="M7.38286 2.17623C7.49013 2.25964 7.49013 2.41127 7.49013 2.71454L7.49013 16.9322C7.49013 17.1073 7.49013 17.1949 7.4496 17.2615C7.40908 17.328 7.32527 17.3713 7.15764 17.4579C6.93883 17.571 6.70433 17.707 6.44218 17.8591C5.86901 18.1916 5.33959 18.4987 4.94522 18.6768C4.54633 18.8569 4.1136 18.9939 3.65702 18.9124C3.16317 18.8242 2.71978 18.559 2.40525 18.1687C2.11675 17.8108 2.02093 17.3679 1.97753 16.9271C1.93455 16.4905 1.93456 15.9323 1.93457 15.2572V7.62844C1.93456 7.11813 1.93455 6.68632 1.96926 6.32858C2.00612 5.94867 2.08587 5.59808 2.27714 5.26233C2.46888 4.92577 2.72946 4.67994 3.0372 4.45772C3.32574 4.24936 3.69441 4.03551 4.12781 3.78411L5.3209 3.092C5.92005 2.74443 6.41073 2.45977 6.83743 2.2627C7.09197 2.14514 7.21924 2.08636 7.33994 2.14877C7.35384 2.15596 7.3705 2.16662 7.38286 2.17623Z"
                    fill="#B69265"
                  />
                  <path
                    d="M21.7339 11.8561C21.842 11.8066 21.842 11.5969 21.842 11.1776V9.19442C21.842 8.34262 21.842 7.63533 21.7682 7.07447C21.6907 6.48477 21.5205 5.95339 21.1015 5.52512C20.6804 5.09474 20.1544 4.91799 19.5704 4.83772C19.0186 4.76191 18.324 4.76193 17.4928 4.76196L15.5192 4.76196C15.4316 4.76196 15.3605 4.83284 15.3605 4.92036L15.3605 9.66149C15.3605 9.87975 15.3605 9.98887 15.4274 10.0444C15.4943 10.0999 15.6063 10.079 15.8305 10.0371C16.2088 9.96638 16.5944 9.92995 16.9822 9.92995C18.4983 9.92995 19.9813 10.4868 21.1237 11.4724C21.4584 11.7612 21.6258 11.9056 21.7339 11.8561Z"
                    fill="#B69265"
                  />
                </g>
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M16.9812 11.3193C14.3514 11.3193 12.1201 13.4888 12.1201 16.1461C12.1201 17.5123 12.7001 18.6432 13.485 19.5561C14.2635 20.4615 15.2674 21.1819 16.1865 21.7509L16.1985 21.7582L16.2107 21.7651C16.4459 21.8979 16.7114 21.9675 16.9812 21.9675C17.2511 21.9675 17.5166 21.8979 17.7518 21.7651L17.7624 21.7591L17.7729 21.7527C18.6969 21.1873 19.7012 20.4645 20.4794 19.5563C21.2635 18.6412 21.8423 17.5079 21.8423 16.1461C21.8423 13.4888 19.6111 11.3193 16.9812 11.3193ZM16.9729 14.0971C15.9548 14.0971 15.1294 14.9262 15.1294 15.949C15.1294 16.9717 15.9548 17.8008 16.9729 17.8008H16.9895C18.0077 17.8008 18.8331 16.9717 18.8331 15.949C18.8331 14.9262 18.0077 14.0971 16.9895 14.0971H16.9729Z"
                  fill="#B69265"
                />
              </svg>
              <div>
                <div class="text-subtitle-2 text-medium-emphasis mb-1">
                  موقع الملاحظة
                </div>
                <div class="text-body-1">مخيم النور 3 - القطاع الجنوبي</div>
              </div>
            </div>
          </v-card>
        </div>

        <!-- Evidence Files -->
        <div v-if="filesValue.length > 0" class="files-grid">
          <v-row>
            <v-col
              v-for="(file, index) in filesValue"
              :key="file.name"
              cols="12"
              sm="12"
              md="6"
              class="file-item"
            >
              <!-- Image Preview -->

              <!-- Document Preview -->
              <div
                class="file-preview rounded-lg"
                style="background-color: rgba(247, 247, 248, 1)"
              >
                <div class="d-flex align-start justify-space-between pa-4">
                  <div class="d-flex align-center">
                    <v-icon color="primary" size="24" class="ml-2"
                      >mdi-file-document-outline</v-icon
                    >
                    <div>
                      <div class="text-body-2">{{ file.name }}</div>
                      <div class="text-caption text-grey">
                        {{ file.size }}
                      </div>
                    </div>
                  </div>
                  <v-btn
                    variant="text"
                    density="compact"
                    icon
                    @click.stop="removeFile(index)"
                  >
                    <v-icon>mdi-close</v-icon>
                  </v-btn>
                </div>
              </div>
            </v-col>
          </v-row>
        </div>
        <!-- <div class="mb-6">
          <div class="text-h6 font-weight-bold mb-2">
            الأدلة والشواهد البيئية
          </div>
          <div class="d-flex gap-4">
            <v-card
              v-for="(file, index) in existingFiles"
              :key="index"
              elevation="0"
              class="bg-section rounded-lg pa-2"
              width="200"
            >
              <div class="d-flex align-center gap-2">
                <v-icon color="primary" size="24"
                  >mdi-file-document-outline</v-icon
                >
                <div class="text-truncate">الأدلة والشواهد البيئية</div>
              </div>
              <div class="text-caption text-medium-emphasis mt-1">1.2MB</div>
              <v-btn
                color="primary"
                variant="text"
                size="small"
                class="mt-2"
                prepend-icon="mdi-download"
                @click="downloadFile(file)"
              >
                تحميل
              </v-btn>
            </v-card>
          </div>
        </div> -->
      </v-card-text>
    </v-card>

    <!-- Image Preview Dialog -->
    <v-dialog v-model="previewDialog" max-width="800px">
      <v-card>
        <v-card-title class="d-flex justify-space-between align-center pa-4">
          <span>معاينة الملف</span>
          <v-btn
            icon="mdi-close"
            variant="text"
            @click="previewDialog = false"
          ></v-btn>
        </v-card-title>
        <v-card-text class="pa-0">
          <v-img :src="selectedImage?.url" max-height="600" contain>
            <template v-slot:placeholder>
              <div class="d-flex align-center justify-center fill-height">
                <v-progress-circular
                  indeterminate
                  color="primary"
                ></v-progress-circular>
              </div>
            </template>
          </v-img>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";

const router = useRouter();
const route = useRoute();
const loading = ref(false);
const selectedType = ref("general");
const description = ref("");
const location = ref("");
const existingFiles = ref([]);
const previewDialog = ref(false);
const selectedImage = ref(null);

const filesValue = ref([
  {
    name: "ملف 1.jpg",
    size: "1.2MB",
  },
  {
    name: "ملف 2.jpg",
    size: "1.2MB",
  },
]);

const breadcrumbItems = [
  {
    title: "الرئيسية",
    href: "/",
    disabled: false,
  },
  {
    title: "الملاحظات",
    href: "/notes",
    disabled: false,
  },
  {
    title: "عرض الملاحظة",
    disabled: true,
  },
];

const noteTypes = [
  { label: "ملاحظة عامة", value: "general" },
  { label: "مخالفة", value: "violation" },
];

// Status handling
const status = ref("pending"); // pending, accepted, rejected
const statusColor = computed(() => {
  switch (status.value) {
    case "accepted":
      return "success";
    case "rejected":
      return "error";
    default:
      return "warning";
  }
});

const statusText = computed(() => {
  switch (status.value) {
    case "accepted":
      return "مقبولة";
    case "rejected":
      return "مرفوضة";
    default:
      return "قيد المراجعة";
  }
});

const formattedDate = ref("12 يناير 2024 | 12:24 مساءً");

onMounted(async () => {
  const noteId = route.params.id;
  try {
    // Fetch note data here
    // For now using mock data
    selectedType.value = "general";
    description.value =
      "تم رصد انقطاع التيار الكهربائي في مخيم النور 3 منذ أكثر من ساعتين، مما تسبب في تعطل أجهزة";
    location.value = "مخيم النور 3";
    status.value = "pending";
    existingFiles.value = [
      { url: "/mock-image-1.jpg", size: "1.2MB" },
      { url: "/mock-image-2.jpg", size: "1.2MB" },
    ];
  } catch (error) {
    console.error("Error fetching note:", error);
  }
});

const previewImage = (file) => {
  selectedImage.value = file;
  previewDialog.value = true;
};

const downloadFile = (file) => {
  // Add download logic here
  console.log("Downloading file:", file);
};

const handleApprove = async () => {
  loading.value = true;
  try {
    // Add your approve logic here
    await new Promise((resolve) => setTimeout(resolve, 1000));
    status.value = "accepted";
    // Optionally navigate back or show success message
  } catch (error) {
    console.error("Error approving note:", error);
  } finally {
    loading.value = false;
  }
};

const handleReject = async () => {
  loading.value = true;
  try {
    // Add your reject logic here
    await new Promise((resolve) => setTimeout(resolve, 1000));
    status.value = "rejected";
    // Optionally navigate back or show success message
  } catch (error) {
    console.error("Error rejecting note:", error);
  } finally {
    loading.value = false;
  }
};
</script>

<style lang="scss" scoped>
.border-dashed {
  border: 2px dashed rgba(182, 146, 101, 0.2);
  background-color: #f8f9f9;
  cursor: pointer;
  transition: all 0.3s ease;

  &:hover {
    border-color: rgba(182, 146, 101, 0.4);
  }
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
}

:deep(.v-field) {
  border-radius: 8px !important;
  background-color: #f8f9f9 !important;

  &.v-field--focused {
    background-color: white !important;
  }
}

.custom-breadcrumbs {
  :deep(.v-breadcrumbs-item--disabled) {
    opacity: 1;
  }

  :deep(.v-breadcrumbs__item) {
    color: rgba(0, 0, 0, 0.6);
    font-size: 13px;
    padding: 0 8px;
  }

  :deep(.v-breadcrumbs-item--disabled) {
    color: rgba(0, 0, 0, 0.6);
  }
}

.text-13 {
  font-size: 13px;
}

.bg-section {
  background-color: rgba(249, 249, 249, 1) !important;
}

:deep(.v-card) {
  --v-theme-primary: #b69265;
  --v-theme-on-primary: #ffffff;
}

:deep(.v-field) {
  border-radius: 8px !important;
  background-color: rgba(249, 249, 249, 1) !important;

  &.v-field--focused {
    background-color: white !important;
  }
}
</style>
