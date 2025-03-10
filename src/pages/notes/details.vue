<template>
  <v-container fluid class="pa-0">
    <!-- Breadcrumb -->
    <div class="d-flex align-center mb-4 px-6">
      <router-link
        to="/"
        class="text-decoration-none d-flex align-center text-medium-emphasis"
      >
        <v-icon size="20" class="me-2">mdi-home</v-icon>
      </router-link>

      <v-icon size="18" color="grey" class="mx-2">mdi-chevron-left</v-icon>

      <router-link
        to="/notes"
        class="text-decoration-none d-flex align-center text-medium-emphasis"
      >
        إدارة للملاحظات
      </router-link>

      <v-icon size="18" color="grey" class="mx-2">mdi-chevron-left</v-icon>

      <span class="text-medium-emphasis">نوع الملاحظة</span>
    </div>

    <v-row class="mt-5">
      <v-col cols="12" class="pa-0">
        <v-card elevation="0" class="rounded-lg">
          <v-card-title
            class="d-flex justify-space-between align-start py-4 px-6"
          >
            <h2 class="text-h6 font-weight-bold">
              <div>نوع الملاحظة</div>
              <div class="text-body-2 mt-4 text-medium-emphasis">
                {{ formatDate(note?.created_at) }}
              </div>
            </h2>

            <div class="d-flex align-center gap-2">
              <!-- Status Badge -->
              <!-- <div class="status-badge d-flex align-center gap-2">
                <div class="status-icon">
                  <v-icon
                    v-if="note?.status === 'APPROVED'"
                    color="success"
                    size="20"
                  >
                    mdi-check-circle
                  </v-icon>
                  <v-icon
                    v-else-if="note?.status === 'REJECTED'"
                    color="error"
                    size="20"
                  >
                    mdi-close-circle
                  </v-icon>
                  <v-icon v-else color="warning" size="20">
                    mdi-clock-outline
                  </v-icon>
                </div>
                <span class="status-text" :class="statusTextClass">
                  {{ getStatusText(note?.status) }}
                </span>
              </div>
              <span class="text-body-2 text-medium-emphasis me-2">
                {{ formatDate(note?.created_at) }}
              </span> -->

              <div
                v-if="
                  ['REJECTED', 'PENDING', 'APPROVED'].includes(note?.status)
                "
                class="action-buttons-container"
              >
                <v-btn
                  v-if="['REJECTED', 'PENDING'].includes(note?.status)"
                  color="success"
                  variant="tonal"
                  size="small"
                  class="text-none font-weight-medium px-6 py-2 rounded-pill"
                  @click="updateStatus('APPROVED')"
                  :loading="loading"
                >
                  <span class="me-2">
                    {{
                      note?.status === "REJECTED" ? "تراجع عن الرفض" : "قبول"
                    }}
                  </span>
                  <v-icon size="small">mdi-check</v-icon>
                </v-btn>

                <v-btn
                  v-if="['APPROVED', 'PENDING'].includes(note?.status)"
                  color="error"
                  variant="tonal"
                  size="small"
                  class="text-none font-weight-medium px-6 py-2 rounded-pill"
                  @click="showRejectDialog = true"
                  :loading="loading"
                >
                  <span class="me-2">
                    {{
                      note?.status === "APPROVED" ? "تراجع عن القبول" : "رفض"
                    }}
                  </span>
                  <v-icon size="small">mdi-close</v-icon>
                </v-btn>
              </div>
            </div>
          </v-card-title>

          <v-divider></v-divider>

          <v-card-text class="px-6 py-4">
            <!-- Loading State -->
            <v-row v-if="loading">
              <v-col cols="12">
                <v-skeleton-loader type="article"></v-skeleton-loader>
              </v-col>
            </v-row>

            <!-- Error State -->
            <v-row v-else-if="error">
              <v-col cols="12" class="text-center">
                <p class="text-error">{{ error }}</p>
              </v-col>
            </v-row>

            <!-- Content -->
            <v-row v-else>
              <v-col cols="12">
                <!-- Description Section -->
                <div class="section-container mb-6">
                  <div class="section-title mb-4">وصف الملاحظة</div>
                  <p class="text-body-1 bg-gray-1 pa-4 rounded-lg">
                    {{ note?.description }}
                  </p>
                </div>

                <div class="section-title mb-4">تفاصيل الملاحظة</div>

                <!-- Note Details -->
                <div class="note-details rounded-lg pa-6 mb-6">
                  <!-- Reporter Info -->

                  <v-row class="mb-5 align-center">
                    <v-col cols="12" md="6">
                      <!-- Date and Time -->
                      <div class="info-section">
                        <div class="d-flex align-center mb-2">
                          <svg
                            width="23"
                            height="26"
                            viewBox="0 0 23 26"
                            fill="none"
                            xmlns="http://www.w3.org/2000/svg"
                          >
                            <path
                              opacity="0.4"
                              d="M20.6854 14.714V14.2385C20.6854 12.6372 20.6854 11.3537 20.6129 10.3096H3.16526C3.09277 11.3537 3.09277 12.6372 3.09277 14.2385V14.714C3.09277 18.7485 3.09277 20.7658 4.25214 22.0192C5.41151 23.2725 7.27749 23.2725 11.0094 23.2725H12.7687C16.5007 23.2725 18.3666 23.2725 19.526 22.0192C20.6854 20.7658 20.6854 18.7485 20.6854 14.714Z"
                              fill="#B69265"
                            />
                            <path
                              d="M7.25955 14.9392C7.25955 14.4278 7.6741 14.0133 8.18547 14.0133H8.19378C8.70516 14.0133 9.11971 14.4278 9.11971 14.9392C9.11971 15.4506 8.70516 15.8651 8.19378 15.8651H8.18547C7.6741 15.8651 7.25955 15.4506 7.25955 14.9392Z"
                              fill="#B69265"
                            />
                            <path
                              d="M10.9591 14.9392C10.9591 14.4278 11.3736 14.0133 11.885 14.0133H11.8933C12.4047 14.0133 12.8193 14.4278 12.8193 14.9392C12.8193 15.4506 12.4047 15.8651 11.8933 15.8651H11.885C11.3736 15.8651 10.9591 15.4506 10.9591 14.9392Z"
                              fill="#B69265"
                            />
                            <path
                              d="M14.6586 14.9392C14.6586 14.4278 15.0732 14.0133 15.5846 14.0133H15.5929C16.1043 14.0133 16.5188 14.4278 16.5188 14.9392C16.5188 15.4506 16.1043 15.8651 15.5929 15.8651H15.5846C15.0732 15.8651 14.6586 15.4506 14.6586 14.9392Z"
                              fill="#B69265"
                            />
                            <path
                              d="M7.25955 18.6429C7.25955 18.1315 7.6741 17.717 8.18547 17.717H8.19378C8.70516 17.717 9.11971 18.1315 9.11971 18.6429C9.11971 19.1543 8.70516 19.5688 8.19378 19.5688H8.18547C7.6741 19.5688 7.25955 19.1543 7.25955 18.6429Z"
                              fill="#B69265"
                            />
                            <path
                              d="M10.9591 18.6429C10.9591 18.1315 11.3736 17.717 11.885 17.717H11.8933C12.4047 17.717 12.8193 18.1315 12.8193 18.6429C12.8193 19.1543 12.4047 19.5688 11.8933 19.5688H11.885C11.3736 19.5688 10.9591 19.1543 10.9591 18.6429Z"
                              fill="#B69265"
                            />
                            <path
                              fill-rule="evenodd"
                              clip-rule="evenodd"
                              d="M6.33362 4.05957C6.71715 4.05957 7.02807 4.37048 7.02807 4.75401V5.09281C8.07998 4.98547 9.37439 4.98548 10.9533 4.9855H12.8251C14.404 4.98548 15.6984 4.98547 16.7503 5.09281V4.75401C16.7503 4.37048 17.0612 4.05957 17.4447 4.05957C17.8283 4.05957 18.1392 4.37048 18.1392 4.75401V5.34178C18.8843 5.55298 19.5128 5.89624 20.0359 6.46176C20.7562 7.24045 21.0764 8.22431 21.2297 9.45726C21.3799 10.6654 21.3799 12.2137 21.3799 14.1893V14.7632C21.3799 16.7388 21.3799 18.2871 21.2297 19.4952C21.0764 20.7282 20.7562 21.712 20.0359 22.4907C19.3085 23.2771 18.3774 23.6337 17.2121 23.8031C16.0843 23.967 14.6433 23.967 12.8251 23.967H10.9533C9.13506 23.967 7.6941 23.967 6.56629 23.8031C5.401 23.6337 4.46987 23.2771 3.74246 22.4907C3.02217 21.712 2.702 20.7282 2.54867 19.4952C2.39842 18.2871 2.39843 16.7388 2.39844 14.7632V14.1892C2.39843 12.2137 2.39842 10.6654 2.54867 9.45726C2.702 8.22431 3.02217 7.24045 3.74246 6.46176C4.26557 5.89624 4.89403 5.55298 5.63918 5.34178V4.75401C5.63918 4.37048 5.95009 4.05957 6.33362 4.05957ZM6.33362 7.30031C6.01619 7.30031 5.7485 7.08733 5.66566 6.79646C5.28852 6.94652 5.00217 7.14527 4.76204 7.40487C4.32477 7.8776 4.06559 8.52564 3.92863 9.61513H19.8497C19.7128 8.52564 19.4536 7.8776 19.0163 7.40487C18.7762 7.14527 18.4898 6.94653 18.1127 6.79647C18.0299 7.08733 17.7622 7.30031 17.4447 7.30031C17.0612 7.30031 16.7503 6.9894 16.7503 6.60587V6.48974C15.7679 6.37583 14.4981 6.37439 12.7688 6.37439H11.0095C9.28026 6.37439 8.01048 6.37583 7.02807 6.48974V6.60587C7.02807 6.9894 6.71715 7.30031 6.33362 7.30031ZM19.955 11.004C19.9905 11.89 19.991 12.9492 19.991 14.2385V14.714C19.991 16.7494 19.9898 18.2114 19.8514 19.3238C19.7149 20.4215 19.4554 21.0729 19.0163 21.5476C18.5844 22.0146 18.0028 22.2847 17.0123 22.4286C15.9959 22.5764 14.656 22.5781 12.7688 22.5781H11.0095C9.1224 22.5781 7.78247 22.5764 6.76607 22.4286C5.77555 22.2847 5.19401 22.0146 4.76204 21.5476C4.32296 21.0729 4.06345 20.4215 3.92694 19.3238C3.78859 18.2114 3.78733 16.7494 3.78733 14.714V14.2385C3.78733 12.9492 3.78783 11.89 3.82337 11.004H19.955Z"
                              fill="#B69265"
                            />
                          </svg>

                          <span class="section-subtitle ms-2"
                            >تاريخ ووقت الملاحظة</span
                          >
                        </div>
                        <div class="text-body-1">
                          {{ formatDate(note?.report_date) }}
                        </div>
                      </div>
                    </v-col>
                    <v-col cols="12" md="6">
                      <div class="d-flex align-center">
                        <div class="info-section">
                          <div class="d-flex align-center mb-2">
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

                            <span class="section-subtitle ms-2"
                              >مرسل الملاحظة</span
                            >
                          </div>
                          <v-avatar v-if="note?.report_by?.avatar" size="40">
                            <v-img
                              :src="note?.report_by?.avatar"
                              cover
                              class="rounded-circle"
                            ></v-img>
                          </v-avatar>
                          <div class="text-subtitle-1 font-weight-bold">
                            {{ note?.report_by?.firstname }}
                            {{ note?.report_by?.lastname }}
                          </div>
                          <div class="text-caption text-medium-emphasis">
                            {{ note?.report_by?.email }}
                          </div>
                        </div>
                      </div>
                    </v-col>
                  </v-row>

                  <!-- Location -->
                  <div class="info-section mb-6">
                    <div class="d-flex align-center mb-2">
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

                      <span class="section-subtitle ms-2">موقع الملاحظة</span>
                    </div>
                    <div class="text-body-1 mb-4">{{ note?.address }}</div>

                    <!-- Map -->
                    <div
                      v-if="note?.latitude && note?.longitude"
                      class="map-container rounded-lg overflow-hidden"
                    >
                      <LocationPicker
                        v-model="selectedLocation"
                        :readonly="true"
                        :height="300"
                        :hideCoordinates="true"
                        class="rounded-lg"
                      />
                    </div>
                  </div>

                  <!-- Attachments -->
                  <div class="info-section" v-if="note?.attachments?.length">
                    <div class="d-flex align-center mb-3">
                      <v-icon size="20" color="primary" class="me-2"
                        >mdi-paperclip</v-icon
                      >
                      <span class="section-subtitle"
                        >الأدلة والشواهد الميدانية</span
                      >
                    </div>
                    <div class="attachments-grid me-8">
                      <v-card
                        v-for="(attachment, index) in note.attachments"
                        :key="index"
                        elevation="0"
                        class="attachment-card"
                      >
                        <v-img
                          :src="attachment.url"
                          height="120"
                          cover
                          class="rounded"
                        ></v-img>
                        <div class="text-caption mt-2 text-center">
                          {{ attachment.size }}
                        </div>
                      </v-card>
                    </div>
                  </div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Reject Dialog -->
    <v-dialog v-model="showRejectDialog" max-width="500" class="reject-dialog">
      <v-card class="rounded-lg">
        <v-card-title class="text-h6 font-weight-bold pa-6">
          رفض الملاحظة
        </v-card-title>
        <v-card-text class="px-6 pb-6">
          <v-textarea
            v-model="rejectionReason"
            label="سبب الرفض"
            placeholder="يرجى إدخال سبب الرفض"
            rows="3"
            hide-details
            class="mb-0 rounded-lg"
            variant="outlined"
            :rules="[(v) => !!v || 'سبب الرفض مطلوب']"
            required
          ></v-textarea>
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer></v-spacer>
          <v-btn
            color="default"
            variant="text"
            class="me-3"
            @click="
              () => {
                showRejectDialog = false;
                rejectionReason = '';
              }
            "
          >
            إلغاء
          </v-btn>
          <v-btn
            color="error"
            variant="tonal"
            @click="updateStatus('REJECTED')"
            :loading="loading"
            :disabled="!rejectionReason"
            class="px-6"
          >
            تأكيد الرفض
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import axios from "@/plugins/axios";
import { useAppStore } from "@/stores/app";
import LocationPicker from "@/components/maps/LocationPicker.vue";

const route = useRoute();
const router = useRouter();
const appStore = useAppStore();

const note = ref(null);
const loading = ref(true);
const error = ref(null);
const showRejectDialog = ref(false);
const rejectionReason = ref("");
const selectedLocation = ref(null);

// Format date and time
const formatDateTime = (date) => {
  if (!date) return "";
  const dateObj = new Date(date);
  const formatter = new Intl.DateTimeFormat("ar", {
    year: "numeric",
    month: "numeric",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    hour12: true,
  });
  return formatter.format(dateObj);
};

// Get initials from name
const getInitials = (firstName, lastName) => {
  return `${firstName?.charAt(0) || ""}${
    lastName?.charAt(0) || ""
  }`.toUpperCase();
};

// Update location when note data is loaded
const updateLocation = () => {
  if (note.value?.latitude && note.value?.longitude) {
    selectedLocation.value = {
      lat: parseFloat(note.value.latitude),
      lng: parseFloat(note.value.longitude),
    };
  }
};

// Fetch note details
const fetchNoteDetails = async () => {
  try {
    loading.value = true;
    error.value = null;
    const response = await axios.get(`/reports/${route.params.id}`);
    note.value = response.data.data;
    updateLocation(); // Update location after fetching note details
  } catch (err) {
    console.error("Error fetching note details:", err);
    error.value = "حدث خطأ أثناء جلب تفاصيل الملاحظة";
    appStore.showSnackbar({
      message: error.value,
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Update status
const updateStatus = async (newStatus) => {
  try {
    // Check for rejection reason before making the request
    if (newStatus === "REJECTED" && !rejectionReason.value) {
      appStore.showSnackbar({
        message: "يرجى إدخال سبب الرفض",
        color: "error",
      });
      return;
    }

    loading.value = true;
    const payload = {
      status: newStatus,
      ...(newStatus === "REJECTED" && {
        rejection_reason: rejectionReason.value,
      }),
    };

    await axios.patch(`/reports/${route.params.id}/status`, payload);

    showRejectDialog.value = false;
    rejectionReason.value = "";

    appStore.showSnackbar({
      message:
        newStatus === "APPROVED"
          ? "تم قبول الملاحظة بنجاح"
          : "تم رفض الملاحظة بنجاح",
      color: "success",
    });

    await fetchNoteDetails();
  } catch (err) {
    console.error("Error updating note status:", err);
    appStore.showSnackbar({
      message: "حدث خطأ أثناء تحديث حالة الملاحظة",
      color: "error",
    });
  } finally {
    loading.value = false;
  }
};

// Format date using the same format as CorrectSiteCard
const formatDate = (dateString) => {
  if (!dateString) return "غير محدد";

  return new Date(dateString).toLocaleDateString("ar", {
    year: "numeric",
    month: "long",
    day: "numeric",
  });
};

// Get status text
const getStatusText = (status) => {
  const statusMap = {
    APPROVED: "معتمد",
    REJECTED: "مرفوض",
    PENDING: "قيد المراجعة",
  };
  return statusMap[status] || "قيد المراجعة";
};

// Status text class
const statusTextClass = computed(() => {
  const classMap = {
    APPROVED: "text-success",
    REJECTED: "text-error",
    PENDING: "text-warning",
  };
  return classMap[note.value?.status] || "text-warning";
});

onMounted(() => {
  fetchNoteDetails();
});
</script>

<style lang="scss" scoped>
.note-details {
  background-color: #f8f9f9;
}

.section-container {
  // background-color: white;
  // border-radius: 12px;
  // padding: 1.5rem;
  // box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.section-title {
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 1rem;
}

.section-subtitle {
  font-size: 0.95rem;
  font-weight: 500;
  color: #666;
}

.info-section {
  &:not(:last-child) {
    border-bottom: 1px solid rgba(0, 0, 0, 0.08);
    padding-bottom: 1.5rem;
  }
}

.map-container {
  border: 1px solid rgba(0, 0, 0, 0.12);
  background-color: white;
}

.attachments-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
  margin-top: 1rem;
}

.attachment-card {
  background-color: white;
  border: 1px solid rgba(0, 0, 0, 0.12);
  border-radius: 8px;
  overflow: hidden;
  transition: transform 0.2s ease;

  &:hover {
    transform: translateY(-2px);
  }
}

.action-buttons-container {
  display: flex;
  gap: 1rem;
  // margin-bottom: 2rem;
  // padding: 0.5rem;
  // border-radius: 12px;
  // background-color: white;
  // box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

:deep(.v-btn) {
  text-transform: none;
  letter-spacing: 0;
  font-weight: 500;
}

.reject-dialog {
  :deep(.v-card) {
    .v-card-title {
      font-size: 1.1rem;
    }

    .v-textarea {
      .v-field {
        border-radius: 8px;
        background-color: rgb(250, 250, 250);

        &--focused {
          background-color: white;
        }
      }
    }
  }
}

// Status styles from CorrectSiteCard
.status-badge {
  .status-icon {
    display: flex;
    align-items: center;
  }

  .status-text {
    font-weight: 500;
    font-size: 0.9rem;
  }
}

.text-success {
  color: #65b669;
}

.text-error {
  color: #f53d6b;
}

.text-warning {
  color: #fb8c00;
}

.text-decoration-none {
  text-decoration: none;
  color: inherit;
  font-size: 0.875rem;

  &:hover {
    color: var(--v-primary-base);
  }
}
</style>
