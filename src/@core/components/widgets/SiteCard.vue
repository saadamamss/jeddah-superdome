<template>
  <v-card elevation="0" class="site-card rounded-lg">
    <v-card-text class="pa-4">
      <div class="d-flex justify-space-between align-center mb-4">
        <div class="d-flex align-center justify-space-between w-100">
          <div
            class="font-weight-medium d-flex align-center ga-1"
            style="font-size: 0.9rem; max-width: 170px"
          >
            <span v-if="status == 'active'">
              <svg
                width="15"
                height="16"
                viewBox="0 0 15 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M0.147461 8.00004C0.147461 11.958 3.35613 15.1667 7.31413 15.1667C11.2721 15.1667 14.4808 11.958 14.4808 8.00004C14.4808 4.04204 11.2721 0.833374 7.31413 0.833374C3.35613 0.833374 0.147461 4.04204 0.147461 8.00004Z"
                  fill="#65B669"
                />
                <path
                  d="M7.31411 9.33331C7.13744 9.33331 6.96812 9.2633 6.84279 9.13797L4.50945 6.80463C4.24945 6.54397 4.24945 6.12199 4.50945 5.86199C4.77012 5.60132 5.1921 5.60132 5.4521 5.86199L7.28878 7.69864L12.8188 1.55398C13.0648 1.28065 13.4868 1.25797 13.7601 1.50463C14.0341 1.75063 14.0561 2.17264 13.8095 2.44598L7.80945 9.11264C7.68679 9.24864 7.51409 9.32798 7.33143 9.33331C7.32543 9.33331 7.31944 9.33331 7.31411 9.33331Z"
                  fill="#65B669"
                />
              </svg>
            </span>
            <span class="flex-shrink-0" v-else>
              <svg
                width="16"
                height="16"
                viewBox="0 0 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M0.833008 8.00008C0.833008 11.9581 4.04163 15.1667 7.99967 15.1667C11.9577 15.1667 15.1663 11.9581 15.1663 8.00008C15.1663 4.04204 11.9577 0.833414 7.99967 0.833414C4.04163 0.833414 0.833008 4.04204 0.833008 8.00008Z"
                  fill="#F53D6B"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M10.4711 5.52865C10.7314 5.78901 10.7314 6.21112 10.4711 6.47146L8.94238 8.00004L10.4711 9.52862C10.7314 9.78896 10.7314 10.2111 10.4711 10.4714C10.2108 10.7318 9.78864 10.7318 9.52829 10.4715L7.99954 8.94282L6.47106 10.4712C6.21071 10.7315 5.7886 10.7315 5.52825 10.4712C5.26791 10.2108 5.26793 9.7887 5.52829 9.52836L7.0567 8.00004L5.52829 6.47172C5.26793 6.21138 5.26791 5.78927 5.52825 5.52891C5.7886 5.26856 6.21071 5.26854 6.47106 5.52888L7.99954 7.05726L9.52829 5.52862C9.78864 5.26828 10.2108 5.26829 10.4711 5.52865Z"
                  fill="#F53D6B"
                />
              </svg>
            </span>
            <span class="mt-n2 text-truncate">{{ name }}</span>
          </div>

          <div class="d-flex align-center ga-2 flex-shrink-0">
            <v-btn
              v-if="status == 'active' && !isCorrection"
              @click="dialog = true"
              icon
              variant="text"
              size="small"
            >
              <svg
                width="17"
                height="16"
                viewBox="0 0 17 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  opacity="0.4"
                  d="M1.5 7.28389C1.5 3.73973 4.48391 0.833374 8.00197 0.833374C11.2879 0.833374 14.1078 3.36881 14.4657 6.59123C14.4878 6.79097 14.4989 6.89084 14.4376 6.9365C14.3763 6.98217 14.2764 6.93981 14.0768 6.85508C13.5379 6.62643 12.9488 6.50004 12.3366 6.50004C11.8496 6.50004 11.3773 6.58 10.9333 6.72777C10.7882 6.77608 10.7156 6.80023 10.6659 6.77496C10.6162 6.74968 10.5922 6.67431 10.5442 6.52357C10.2013 5.44674 9.19276 4.66671 8.00197 4.66671C6.52876 4.66671 5.33449 5.86061 5.33449 7.33337C5.33449 8.67601 6.32705 9.78689 7.61865 9.97271C7.77398 9.99506 7.85165 10.0062 7.88475 10.0507C7.91786 10.0953 7.90656 10.1704 7.88398 10.3208C7.8519 10.5343 7.83525 10.7529 7.83525 10.9753C7.83525 12.264 8.3878 13.3008 9.06009 14.0825C9.12541 14.1584 9.19235 14.2326 9.26063 14.3049C9.36879 14.4196 9.42286 14.4769 9.41502 14.5393C9.40717 14.6017 9.34295 14.6423 9.21452 14.7236C9.10298 14.7942 8.99179 14.8633 8.88121 14.931L8.87369 14.9356L8.86601 14.9399C8.60245 15.0887 8.3047 15.1667 8.00197 15.1667C7.69923 15.1667 7.40149 15.0887 7.13792 14.9399L7.12912 14.935L7.12053 14.9296C5.81353 14.1208 4.41752 13.1152 3.34515 11.8684C2.26814 10.6161 1.5 9.09886 1.5 7.28389Z"
                  fill="#B69265"
                />
                <path
                  fill-rule="evenodd"
                  clip-rule="evenodd"
                  d="M12.333 7.5C10.4395 7.5 8.83301 9.06203 8.83301 10.9753C8.83301 11.9589 9.25059 12.7732 9.81574 13.4305C10.3762 14.0824 11.0991 14.601 11.7608 15.0107L11.7694 15.016L11.7782 15.021C11.9475 15.1166 12.1387 15.1667 12.333 15.1667C12.5273 15.1667 12.7185 15.1166 12.8878 15.021L12.8955 15.0166L12.903 15.0121C13.5683 14.6049 14.2914 14.0845 14.8517 13.4306C15.4163 12.7718 15.833 11.9558 15.833 10.9753C15.833 9.06203 14.2265 7.5 12.333 7.5ZM12.327 9.66667C11.594 9.66667 10.9997 10.2636 10.9997 11C10.9997 11.7364 11.594 12.3333 12.327 12.3333H12.339C13.0721 12.3333 13.6663 11.7364 13.6663 11C13.6663 10.2636 13.0721 9.66667 12.339 9.66667H12.327Z"
                  fill="#B69265"
                />
              </svg>
            </v-btn>
            <div class="d-flex gap-2">
              <template v-if="isCorrection">
                <v-btn
                  v-if="status == 'rejected'"
                  color="success"
                  variant="outlined"
                  size="small"
                  class="text-none font-weight-medium px-4 rounded-pill"
                  @click="handleApprove"
                  :loading="loading"
                >
                  اعتماد
                </v-btn>
                <v-btn
                  v-else
                  color="error"
                  variant="outlined"
                  size="small"
                  class="text-none font-weight-medium px-4 rounded-pill ms-2"
                  @click="handleReject"
                  :loading="loading"
                >
                  رفض
                </v-btn>
              </template>
              <v-btn
                v-else-if="!isCorrection"
                :to="`/sites/housing-details?siteId=${siteId}`"
                size="small"
                variant="tonal"
                class="text-none font-weight-medium px-2 me-2 bg-primary-100 rounded-pill"
              >
                <span class="me-2"> عرض </span>
                <svg
                  width="17"
                  height="16"
                  viewBox="0 0 17 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    opacity="0.4"
                    fill-rule="evenodd"
                    clip-rule="evenodd"
                    d="M2.66602 7.99992C2.66602 7.63173 2.96449 7.33325 3.33268 7.33325L13.9994 7.33325C14.3675 7.33325 14.666 7.63173 14.666 7.99992C14.666 8.36811 14.3675 8.66659 13.9994 8.66659L3.33268 8.66659C2.96449 8.66659 2.66602 8.36811 2.66602 7.99992Z"
                    fill="#B69265"
                  />
                  <path
                    d="M4.01792 7.99988C4.08012 8.10306 4.21052 8.28847 4.3531 8.44984C4.63693 8.77106 5.02808 9.13798 5.43642 9.49251C5.84144 9.84416 6.24841 10.1708 6.55533 10.4103C6.70844 10.5298 6.97265 10.7294 7.0614 10.7964C7.35783 11.0148 7.42114 11.4321 7.20282 11.7285C6.98448 12.025 6.56714 12.0883 6.27068 11.87L6.2682 11.8681C6.17195 11.7954 5.89287 11.5846 5.73503 11.4614C5.41695 11.2132 4.99059 10.8712 4.56228 10.4993C4.13728 10.1303 3.6951 9.71881 3.35393 9.33269C3.18401 9.14039 3.02506 8.93868 2.90497 8.73948C2.79591 8.55858 2.66603 8.29535 2.66602 7.9999C2.66603 7.70446 2.79591 7.44118 2.90497 7.26028C3.02506 7.06107 3.18401 6.85936 3.35393 6.66707C3.6951 6.28095 4.13728 5.86944 4.56228 5.50044C4.99059 5.12857 5.41695 4.78656 5.73503 4.53832C5.89287 4.41515 6.17177 4.2045 6.26802 4.1318L6.27068 4.12979C6.56714 3.91145 6.98448 3.97479 7.20281 4.27125C7.42114 4.5677 7.35783 4.985 7.0614 5.20335C6.97265 5.27039 6.70844 5.46998 6.55533 5.58946C6.24841 5.82898 5.84144 6.15559 5.43642 6.50725C5.02808 6.86177 4.63693 7.2287 4.3531 7.54992C4.21052 7.71129 4.08012 7.8967 4.01792 7.99988Z"
                    fill="#B69265"
                  />
                </svg>
              </v-btn>
            </div>
          </div>
        </div>
      </div>

      <div class="d-flex align-center mb-2 mt-n4">
        <svg
          width="17"
          height="17"
          viewBox="0 0 17 17"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            opacity="0.4"
            d="M4.87994 1.45131C5.28679 1.36194 5.77226 1.33325 6.33464 1.33325L10.3346 1.33325C10.897 1.33325 11.3825 1.36194 11.7893 1.45131C12.2019 1.54194 12.5648 1.70164 12.8489 1.9857C13.1329 2.26976 13.2926 2.6327 13.3832 3.04522C13.4726 3.45208 13.5013 3.93754 13.5013 4.49992L13.5013 15.1666L3.16797 15.1666L3.16797 4.49992C3.16797 3.93754 3.19665 3.45208 3.28603 3.04522C3.37665 2.6327 3.53635 2.26976 3.82042 1.9857C4.10448 1.70164 4.46742 1.54194 4.87994 1.45131Z"
            fill="#B69265"
          />
          <path
            fill-rule="evenodd"
            clip-rule="evenodd"
            d="M2.00098 15C2.00098 14.6318 2.29945 14.3333 2.66764 14.3333L6.16764 14.3333L6.16764 13C6.16764 12.7154 6.17954 12.4581 6.22016 12.2363C6.26099 12.0132 6.33732 11.788 6.49437 11.5996C6.82043 11.2083 7.3282 11.1667 7.77875 11.1667H8.88987C9.34042 11.1667 9.84819 11.2083 10.1743 11.5996C10.3313 11.788 10.4076 12.0132 10.4485 12.2363C10.4891 12.4581 10.501 12.7154 10.501 13V14.3333L14.001 14.3333C14.3692 14.3333 14.6676 14.6318 14.6676 15C14.6676 15.3682 14.3692 15.6667 14.001 15.6667L2.66764 15.6667C2.29945 15.6667 2.00098 15.3682 2.00098 15ZM9.50098 14.3333H7.16764L7.16764 13C7.16764 12.7331 7.17973 12.5479 7.20381 12.4163C7.22768 12.286 7.25676 12.2467 7.26259 12.2398C7.26495 12.2369 7.26791 12.2338 7.27596 12.2291C7.28541 12.2236 7.30584 12.2136 7.34479 12.2034C7.42915 12.1811 7.56225 12.1667 7.77875 12.1667H8.88987C9.10637 12.1667 9.23947 12.1811 9.32383 12.2034C9.36278 12.2136 9.38321 12.2236 9.39266 12.2291C9.40071 12.2338 9.40367 12.2369 9.40603 12.2398C9.41186 12.2467 9.44094 12.286 9.46481 12.4163C9.48889 12.5479 9.50098 12.7331 9.50098 13L9.50098 14.3333Z"
            fill="#B69265"
          />
          <path
            d="M6.83431 4.5C6.83431 4.22386 7.05817 4 7.33431 4L9.33431 4C9.61045 4 9.83431 4.22386 9.83431 4.5C9.83431 4.77614 9.61045 5 9.33431 5H7.33431C7.05817 5 6.83431 4.77614 6.83431 4.5Z"
            fill="#B69265"
          />
          <path
            d="M6.83431 6.83333C6.83431 6.55719 7.05817 6.33333 7.33431 6.33333H9.33431C9.61045 6.33333 9.83431 6.55719 9.83431 6.83333C9.83431 7.10948 9.61045 7.33333 9.33431 7.33333H7.33431C7.05817 7.33333 6.83431 7.10948 6.83431 6.83333Z"
            fill="#B69265"
          />
          <path
            d="M6.83431 9.16667C6.83431 8.89052 7.05817 8.66667 7.33431 8.66667L9.33431 8.66667C9.61045 8.66667 9.83431 8.89052 9.83431 9.16667C9.83431 9.44281 9.61045 9.66667 9.33431 9.66667L7.33431 9.66667C7.05817 9.66667 6.83431 9.44281 6.83431 9.16667Z"
            fill="#B69265"
          />
        </svg>

        <span class="card-gray-text">{{ companiesCount }} شركه</span>
      </div>

      <div class="site-info mt-6 d-flex align-center justify-space-between">
        <div class="d-flex justify-space-between align-center">
          <div class="d-flex align-start ga-2">
            <svg
              width="16"
              height="18"
              viewBox="0 0 16 18"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                opacity="0.4"
                d="M8.03275 2.83337C9.26153 2.83337 10.2284 2.83336 10.9838 2.92223C11.7543 3.01287 12.3807 3.20288 12.8809 3.6405C13.3906 4.08654 13.6211 4.65971 13.7293 5.3638C13.8327 6.03715 13.8327 6.89384 13.8327 7.95834V12.0417C13.8327 13.1062 13.8327 13.9629 13.7293 14.6363C13.6211 15.3404 13.3906 15.9135 12.8809 16.3596C12.3807 16.7972 11.7543 16.9872 10.9838 17.0779C10.2284 17.1667 9.26152 17.1667 8.03274 17.1667H7.96596C6.73718 17.1667 5.77027 17.1667 5.01487 17.0779C4.24436 16.9872 3.61796 16.7972 3.11781 16.3596C2.60806 15.9135 2.37763 15.3404 2.26945 14.6363C2.16599 13.9629 2.166 13.1062 2.16602 12.0417V12.0417V7.95835V7.95834C2.166 6.89384 2.16599 6.03716 2.26945 5.36381C2.37763 4.65971 2.60806 4.08654 3.11781 3.6405C3.61796 3.20288 4.24436 3.01287 5.01487 2.92223C5.77026 2.83336 6.73717 2.83337 7.96595 2.83337H7.96595H8.03274H8.03275Z"
                fill="#B69265"
              />
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M5.16602 8.71358C5.16602 7.13983 6.45339 5.83337 7.99935 5.83337C9.54531 5.83337 10.8327 7.13983 10.8327 8.71358C10.8327 10.33 9.51364 11.3908 8.50602 12.025L8.49846 12.0297L8.49074 12.0342C8.34164 12.1208 8.17226 12.1667 7.99935 12.1667C7.82644 12.1667 7.65706 12.1208 7.50796 12.0342L7.49911 12.0291L7.49047 12.0236C6.4891 11.386 5.16602 10.3354 5.16602 8.71358ZM6.99935 8.66671C6.99935 8.11442 7.44506 7.66671 7.99488 7.66671H8.00382C8.55364 7.66671 8.99935 8.11442 8.99935 8.66671C8.99935 9.21899 8.55364 9.66671 8.00382 9.66671H7.99488C7.44506 9.66671 6.99935 9.21899 6.99935 8.66671Z"
                fill="#B69265"
              />
              <path
                d="M7.33268 14.1667C7.05654 14.1667 6.83268 14.3906 6.83268 14.6667C6.83268 14.9429 7.05654 15.1667 7.33268 15.1667H8.66602C8.94216 15.1667 9.16602 14.9429 9.16602 14.6667C9.16602 14.3906 8.94216 14.1667 8.66602 14.1667H7.33268Z"
                fill="#B69265"
              />
            </svg>

            <div>
              <div class="text-body-2 text-medium-emphasis">رقم البريد</div>
              <div class="text-body-2 font-weight-medium">{{ licenseNo }}</div>
            </div>
          </div>
        </div>
        <div class="d-flex justify-space-between align-center">
          <div class="d-flex align-start ga-2">
            <svg
              width="17"
              height="16"
              viewBox="0 0 17 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <g opacity="0.4">
                <path
                  d="M2.06292 5.04751C2.24692 5.95969 2.55209 6.85958 3.1841 7.96164L4.88834 6.00405C5.51059 5.28929 5.82174 4.9319 5.86749 4.4931C5.91325 4.0543 5.68255 3.64042 5.22115 2.81265L4.63902 1.76831C4.38118 1.30572 3.88132 1.18429 3.44528 1.53969C2.41356 2.38058 1.79077 3.69843 2.06292 5.04751Z"
                  fill="#B69265"
                />
                <path
                  d="M11.6182 14.6033C10.706 14.4193 9.80617 14.1141 8.7041 13.4821L10.6617 11.7778C11.3764 11.1556 11.7338 10.8444 12.1726 10.7987C12.6114 10.753 13.0253 10.9836 13.8531 11.445L14.8974 12.0272C15.36 12.285 15.4814 12.7849 15.126 13.2209C14.2852 14.2526 12.9673 14.8754 11.6182 14.6033Z"
                  fill="#B69265"
                />
              </g>
              <path
                fill-rule="evenodd"
                clip-rule="evenodd"
                d="M4.21053 0.857753C4.58832 0.935833 4.88919 1.18966 5.07611 1.52501L5.67159 2.59331C5.89093 2.98678 6.07551 3.3179 6.19605 3.60575C6.32392 3.91109 6.39987 4.2122 6.36515 4.54511C6.33043 4.87801 6.19399 5.15697 6.00588 5.42937C5.82854 5.68615 5.57961 5.97207 5.28381 6.31182L3.79857 8.01784C4.97647 9.94851 6.71576 11.6886 8.64842 12.8676L10.3544 11.3824C10.6941 11.0866 10.9801 10.8377 11.2369 10.6604C11.5093 10.4722 11.7882 10.3358 12.1211 10.3011C12.454 10.2664 12.7551 10.3423 13.0605 10.4702C13.3483 10.5907 13.6795 10.7753 14.0729 10.9946L15.1412 11.5901C15.4765 11.777 15.7304 12.0779 15.8085 12.4557C15.8873 12.8374 15.771 13.2211 15.5139 13.5365C14.5813 14.6807 13.0871 15.4093 11.5197 15.0931C10.5562 14.8987 9.60555 14.5749 8.45575 13.9155C6.14549 12.5906 4.07411 10.518 2.75072 8.21051C2.09131 7.06071 1.7675 6.11004 1.57314 5.14653C1.25694 3.57905 1.9855 2.08487 3.12974 1.15226C3.44509 0.895234 3.82879 0.778853 4.21053 0.857753ZM9.57981 13.3826C10.3574 13.7658 11.0337 13.9749 11.7174 14.1128C12.8481 14.3409 13.9896 13.8239 14.7388 12.9047C14.8372 12.784 14.8378 12.6998 14.8291 12.6581C14.8197 12.6125 14.7815 12.5345 14.6543 12.4636L13.61 11.8814C13.1862 11.6452 12.9041 11.4888 12.6742 11.3926C12.4559 11.3012 12.3307 11.2846 12.2248 11.2957C12.1189 11.3067 11.9998 11.3488 11.8051 11.4832C11.6 11.6248 11.3563 11.8361 10.9903 12.1546L9.57981 13.3826ZM3.28359 7.08644L4.51158 5.67588C4.83013 5.30997 5.04139 5.06621 5.18303 4.8611C5.31749 4.66641 5.3595 4.54728 5.37054 4.44139C5.38159 4.3355 5.36506 4.21026 5.27367 3.99202C5.17738 3.76211 5.02097 3.47999 4.78476 3.05623L4.20264 2.01189C4.13172 1.88466 4.05373 1.84648 4.00813 1.83705C3.96645 1.82844 3.88221 1.82905 3.76152 1.92741C2.84233 2.67659 2.32531 3.81809 2.55339 4.94879C2.69131 5.63251 2.90044 6.30883 3.28359 7.08644Z"
                fill="#B69265"
              />
            </svg>

            <div>
              <div class="text-body-2 text-medium-emphasis">رقم المفوض</div>
              <div class="text-body-2 font-weight-medium">
                {{ delegatePhone }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </v-card-text>

    <!-- Location Correction Modal -->
    <v-dialog v-model="dialog" width="700" class="location-dialog">
      <v-card class="rounded-lg">
        <v-card-text class="pa-6">
          <div class="d-flex justify-space-between align-center mb-6">
            <h2 class="text-h6">إضافة موقع جديد</h2>
            <v-btn icon variant="text" @click="dialog = false">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
          <p class="text-subtitle-1 text-medium-emphasis mb-6">
            حدد تفاصيل الموقع الجديد
          </p>
          <v-form @submit.prevent="handleSubmit" ref="form">
            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">نوع الموقع</label>
              <v-select
                v-model="formData.location_type_id"
                :items="siteTypes"
                variant="outlined"
                density="comfortable"
                placeholder="اختر نوع الموقع"
                :rules="[(v) => !!v || 'نوع الموقع مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">اسم المخيم</label>
              <v-text-field
                v-model="formData.name"
                variant="outlined"
                density="comfortable"
                placeholder="ادخل اسم المخيم"
                :rules="[(v) => !!v || 'اسم المخيم مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">رقم المخيم</label>
              <v-text-field
                v-model="formData.camp_number"
                variant="outlined"
                density="comfortable"
                placeholder="ادخل رقم المخيم"
                :rules="[(v) => !!v || 'رقم المخيم مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">رقم البريد</label>
              <v-text-field
                v-model="formData.license_no"
                variant="outlined"
                density="comfortable"
                placeholder="ادخل رقم البريد"
                :rules="[(v) => !!v || 'رقم البريد مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">اسم المفوض</label>
              <v-text-field
                v-model="formData.delegate_name"
                variant="outlined"
                density="comfortable"
                placeholder="ادخل اسم المفوض"
                :rules="[(v) => !!v || 'اسم المفوض مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block">رقم المفوض</label>
              <v-text-field
                v-model="formData.delegate_phone"
                variant="outlined"
                density="comfortable"
                placeholder="ادخل رقم المفوض"
                :rules="[(v) => !!v || 'رقم المفوض مطلوب']"
                hide-details
                class="rounded-lg mb-4"
              />
            </div>

            <div class="mb-4">
              <label class="text-subtitle-2 mb-2 d-block"
                >حدد الموقع على الخريطة</label
              >
              <LocationPicker
                v-model="selectedLocation"
                class="rounded-lg overflow-hidden"
              />
            </div>

            <div class="d-flex justify-end mt-6">
              <v-btn
                variant="text"
                color="grey-darken-1"
                class="me-4"
                @click="dialog = false"
              >
                إلغاء
              </v-btn>
              <v-btn
                color="#B69265"
                class="text-white"
                type="submit"
                :loading="loading"
              >
                حفظ
              </v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script setup>
import { ref, watch } from "vue";
import LocationPicker from "@/components/maps/LocationPicker.vue";
import axiosIns from "@/plugins/axios";
import { useAppStore } from "@/stores/app";

const props = defineProps({
  siteId: {
    type: [String, Number],
    required: true,
  },
  name: {
    type: String,
    required: true,
  },
  licenseNo: {
    type: String,
    required: true,
  },
  delegatePhone: {
    type: String,
    required: true,
  },
  companiesCount: {
    type: Number,
    default: 0,
  },
  locationType: {
    type: Object,
    default: () => ({
      title: "",
      description: "",
    }),
  },
  status: {
    type: String,
    default: "active",
  },
  isCorrection: {
    type: Boolean,
    default: false,
  },
  loading: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["location-created", "approve", "reject", "update"]);

const dialog = ref(false);
const form = ref(null);
const selectedLocation = ref({ lat: 24.7136, lng: 46.6753 }); // Default to Riyadh

const formData = ref({
  name: "",
  location_type_id: null,
  location_sub_type_id: 1,
  contract_id: 3,
  delegate_name: "",
  delegate_phone: "",
  license_no: "",
  camp_number: "",
  square_number: "",
  latitude: "",
  longitude: "",
});

const siteTypes = [
  { title: "موقع سكني", value: 1 },
  { title: "موقع تجاري", value: 2 },
  { title: "موقع صناعي", value: 3 },
];

watch(
  selectedLocation,
  (newLocation) => {
    if (newLocation) {
      formData.value.latitude = newLocation.lat.toString();
      formData.value.longitude = newLocation.lng.toString();
      formData.value.square_number = formData.value.camp_number;
    }
  },
  { deep: true }
);

watch(
  () => formData.value.camp_number,
  (newValue) => {
    if (newValue) {
      formData.value.square_number = newValue;
    }
  }
);

const handleSubmit = async () => {
  const { valid } = await form.value.validate();

  if (!valid) return;

  try {
    const response = await axiosIns.post("locations", {
      ...formData.value,
    });
    console.log("Location created:", response.data);
    dialog.value = false;
    emit("location-created");
  } catch (error) {
    console.error("Error creating location:", error);
  }
};

const handleApprove = () => {
  emit("approve", props.siteId);
};

const handleReject = () => {
  emit("reject", props.siteId);
};

const handleUpdate = () => {
  emit("update", props.siteId);
};
</script>

<style lang="scss" scoped>
.site-card {
  // background-color: rgb(250, 250, 250) !important;
  // border: 1px solid rgba(182, 146, 101, 0.12);
  transition: all 0.3s ease;

  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.04) !important;

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
  }

  .text-h6 {
    font-size: 1.1rem !important;
    line-height: 1.5;
  }

  .site-info {
    background-color: #f9f9f9;
    border-radius: 8px;
    padding: 1rem;
  }
}

.location-dialog {
  :deep(.v-card) {
    background-color: white;
  }

  :deep(.v-field) {
    border-radius: 8px;
    background-color: rgb(250, 250, 250);
  }

  :deep(.v-btn) {
    text-transform: none;
    letter-spacing: 0;
    border-radius: 8px;
  }
}
</style>
