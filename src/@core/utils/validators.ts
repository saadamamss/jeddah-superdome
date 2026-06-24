import i18n from '@/plugins/i18n'
import { isEmpty, isEmptyArray, isNullOrUndefined } from './index'

const { t } = i18n.global

// 👉 Required Validator
export const requiredValidator = (value: unknown) => {
  if (isNullOrUndefined(value) || isEmptyArray(value) || value === false) return t('validators.required')

  return !!String(value).trim().length || t('validators.required')
}

// 👉 Phone Validator
export const phoneValidator = (value: unknown) => {
  if (isNullOrUndefined(value)) return t('validators.required')

  return (String(value).trim().length > 1 && String(value)?.startsWith('05')) || t('validators.phoneStart')
}

// 👉 Email Validator
export const emailValidator = (value: unknown) => {
  if (isEmpty(value)) return true
  const re =
    /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  if (Array.isArray(value)) return value.every((val) => re.test(String(val))) || t('validators.emailInvalid')

  return re.test(String(value)) || t('validators.emailInvalid')
}

// 👉 Password Validator
export const passwordValidator = (password: string) => {
  const regExp = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*()]).{8,}/
  const validPassword = regExp.test(password)

  return (
    // eslint-disable-next-line operator-linebreak
    validPassword || t('validators.passwordStrength')
  )
}

// 👉 Confirm Password Validator
export const confirmPasswordValidator = (value: unknown, target: unknown) =>
  value === target || t('validators.passwordMismatch')

// 👉 Between Validator
export const betweenValidator = (value: unknown, min: number, max: number) => {
  const valueAsNumber = Number(value)

  return (Number(min) <= valueAsNumber && Number(max) >= valueAsNumber) || t('validators.minMax', { min, max })
}

// 👉 Integer Validator
export const integerValidator = (value: unknown) => {
  if (isEmpty(value)) return true
  if (Array.isArray(value)) return value.every((val) => /^-?[0-9]+$/.test(String(val))) || t('validators.integer')

  return /^-?[0-9]+$/.test(String(value)) || t('validators.integer')
}

// 👉 Regex Validator
export const regexValidator = (value: unknown, regex: RegExp | string): boolean | string => {
  if (isEmpty(value)) return true
  const regeX = regex instanceof RegExp ? regex : new RegExp(regex)
  if (Array.isArray(value)) return value.every((val) => regexValidator(val, regeX) === true)

  return regeX.test(String(value)) || t('validators.regex')
}

// 👉 Alpha Validator
export const alphaValidator = (value: unknown) => {
  if (isEmpty(value)) return true

  return /^[A-Z]*$/i.test(String(value)) || t('validators.alpha')
}

// 👉 URL Validator
export const urlValidator = (value: unknown) => {
  if (isEmpty(value)) return true
  const re = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/

  return re.test(String(value)) || t('validators.url')
}

// 👉 Length Validator
export const lengthValidator = (value: unknown, length: number) => {
  if (isEmpty(value)) return true

  return String(value).length === length || t('validators.minLength', { length })
}

// 👉 Alpha-dash Validator
export const alphaDashValidator = (value: unknown) => {
  if (isEmpty(value)) return true
  const valueAsString = String(value)

  return /^[0-9A-Z_-]*$/i.test(valueAsString) || t('validators.alphaNum')
}

export const validationRules = {
  required: (v: unknown) => !!v || t('validators.required'),
  email: (v: string) => {
    if (!v) return true
    return /.+@.+\..+/.test(v) || t('validators.email')
  },
  phone: (v: string) => {
    if (!v) return true
    return /^[0-9]{10}$/.test(v) || t('validators.phoneLength')
  },
  password: (v: string) => {
    if (!v) return true
    return v.length >= 8 || t('validators.passwordLength')
  },
  passwordMatch: (passwordConfirmation: string, password: string) => {
    if (!passwordConfirmation) return true
    return passwordConfirmation === password || t('validators.passwordConfirm')
  },
  saId: (v: string) => {
    if (!v) return true
    return /^[0-9]{10}$/.test(v) || t('validators.identityLength')
  },
}
