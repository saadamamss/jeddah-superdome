import { isEmpty, isEmptyArray, isNullOrUndefined } from './index'

// 👉 Required Validator
export const requiredValidator = value => {
  if (isNullOrUndefined(value) || isEmptyArray(value) || value === false)
    return 'هذا الحقل مطلوب'
  
  return !!String(value).trim().length || 'هذا الحقل مطلوب'
}

// 👉 Email Validator
export const emailValidator = value => {
  if (isEmpty(value))
    return true
  const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
  if (Array.isArray(value))
    return value.every(val => re.test(String(val))) || 'The Email field must be a valid email'
  
  return re.test(String(value)) || 'ادخل قيمة صالحة للبريد الإلكتروني'
}

// 👉 Password Validator
export const passwordValidator = password => {
  const regExp = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%&*()]).{8,}/
  const validPassword = regExp.test(password)
  
  return (
    // eslint-disable-next-line operator-linebreak
    validPassword ||
        'يجب أن يحتوي الحقل على حرف كبير وصغير وحرف خاص ورقم بحد أدنى 8 أحرف')
}

// 👉 Confirm Password Validator
export const confirmedValidator = (value, target) => value === target || 'تأكيد حقل تأكيد كلمة المرور غير متطابق'

// 👉 Between Validator
export const betweenValidator = (value, min, max) => {
  const valueAsNumber = Number(value)
  
  return (Number(min) <= valueAsNumber && Number(max) >= valueAsNumber) || `أدخل الرقم بين ${min} و ${max}`
}

// 👉 Integer Validator
export const integerValidator = value => {
  if (isEmpty(value))
    return true
  if (Array.isArray(value))
    return value.every(val => /^-?[0-9]+$/.test(String(val))) || 'This field must be an integer'
  
  return /^-?[0-9]+$/.test(String(value)) || 'يجب أن يكون هذا الحقل عددًا صحيحًا'
}

// 👉 Regex Validator
export const regexValidator = (value, regex) => {
  if (isEmpty(value))
    return true
  let regeX = regex
  if (typeof regeX === 'string')
    regeX = new RegExp(regeX)
  if (Array.isArray(value))
    return value.every(val => regexValidator(val, regeX))
  
  return regeX.test(String(value)) || 'تنسيق حقل Regex غير صالح'
}

// 👉 Alpha Validator
export const alphaValidator = value => {
  if (isEmpty(value))
    return true
  
  return /^[A-Z]*$/i.test(String(value)) || 'قد يحتوي حقل Alpha على أحرف أبجدية فقط'
}

// 👉 URL Validator
export const urlValidator = value => {
  if (isEmpty(value))
    return true
  const re = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/
  
  return re.test(String(value)) || 'عنوان URL غير صالح'
}

// 👉 Length Validator
export const lengthValidator = (value, length) => {
  if (isEmpty(value))
    return true
  
  return String(value).length === length || ` يجب أن يحتوي الحقل على الأقل علي${length}حروف `
}

// 👉 Alpha-dash Validator
export const alphaDashValidator = value => {
  if (isEmpty(value))
    return true
  const valueAsString = String(value)
  
  return /^[0-9A-Z_-]*$/i.test(valueAsString) || 'جميع الحروف غير صالحة'
}
