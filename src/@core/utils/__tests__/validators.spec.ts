import { describe, expect, it, vi } from 'vitest'

import {
  alphaDashValidator,
  alphaValidator,
  betweenValidator,
  confirmPasswordValidator,
  emailValidator,
  integerValidator,
  lengthValidator,
  passwordValidator,
  phoneValidator,
  regexValidator,
  requiredValidator,
  urlValidator,
  validationRules,
} from '../validators'

vi.mock('@/plugins/i18n', () => ({
  default: {
    global: {
      t: (key: string, params?: Record<string, unknown>) => {
        const messages: Record<string, string> = {
          'validators.required': 'هذا الحقل مطلوب',
          'validators.phoneStart': 'يجب أن يبدأ الرقم بـ 05',
          'validators.emailInvalid': 'البريد الإلكتروني غير صالح',
          'validators.passwordStrength':
            'كلمة المرور يجب أن تحتوي على 8 أحرف على الأقل مع أرقام وحروف كبيرة وصغيرة ورمز خاص',
          'validators.passwordMismatch': 'كلمتا المرور غير متطابقتين',
          'validators.minMax': `يجب أن يكون بين ${params?.min} و ${params?.max}`,
          'validators.integer': 'يجب أن يكون عدد صحيح',
          'validators.regex': 'القيمة غير صالحة',
          'validators.alpha': 'يجب أن يحتوي على حروف فقط',
          'validators.url': 'رابط غير صالح',
          'validators.minLength': `يجب أن يكون الطول ${params?.length}`,
          'validators.alphaNum': 'يجب أن يحتوي على أحرف وأرقام فقط',
          'validators.email': 'البريد الإلكتروني غير صالح',
          'validators.phoneLength': 'يجب أن يحتوي على 10 أرقام',
          'validators.passwordLength': 'يجب أن تكون 8 أحرف على الأقل',
          'validators.passwordConfirm': 'كلمتا المرور غير متطابقتين',
          'validators.identityLength': 'يجب أن يكون 10 أرقام',
        }
        return messages[key] || key
      },
    },
  },
}))

describe('requiredValidator', () => {
  it('returns error for null', () => {
    expect(requiredValidator(null)).toBe('هذا الحقل مطلوب')
  })

  it('returns error for undefined', () => {
    expect(requiredValidator(undefined)).toBe('هذا الحقل مطلوب')
  })

  it('returns error for empty array', () => {
    expect(requiredValidator([])).toBe('هذا الحقل مطلوب')
  })

  it('returns error for false', () => {
    expect(requiredValidator(false)).toBe('هذا الحقل مطلوب')
  })

  it('returns error for empty string', () => {
    expect(requiredValidator('')).toBe('هذا الحقل مطلوب')
  })

  it('returns true for non-empty string', () => {
    expect(requiredValidator('hello')).toBe(true)
  })

  it('returns true for number', () => {
    expect(requiredValidator(42)).toBe(true)
  })

  it('returns true for non-empty array', () => {
    expect(requiredValidator([1, 2])).toBe(true)
  })
})

describe('phoneValidator', () => {
  it('returns error for null', () => {
    expect(phoneValidator(null)).toBe('هذا الحقل مطلوب')
  })

  it('returns true for valid phone starting with 05', () => {
    expect(phoneValidator('0512345678')).toBe(true)
  })

  it('returns error for phone not starting with 05', () => {
    expect(phoneValidator('0123456789')).toBe('يجب أن يبدأ الرقم بـ 05')
  })

  it('returns true for short phone starting with 05', () => {
    expect(phoneValidator('05')).toBe(true)
  })
})

describe('emailValidator', () => {
  it('returns true for empty value', () => {
    expect(emailValidator('')).toBe(true)
  })

  it('returns true for valid email', () => {
    expect(emailValidator('test@example.com')).toBe(true)
  })

  it('returns error for invalid email', () => {
    expect(emailValidator('invalid-email')).toBe('البريد الإلكتروني غير صالح')
  })

  it('returns error for email without domain', () => {
    expect(emailValidator('user@')).toBe('البريد الإلكتروني غير صالح')
  })
})

describe('passwordValidator', () => {
  it('returns error for weak password', () => {
    expect(passwordValidator('weak')).toBe(
      'كلمة المرور يجب أن تحتوي على 8 أحرف على الأقل مع أرقام وحروف كبيرة وصغيرة ورمز خاص',
    )
  })

  it('returns true for strong password', () => {
    expect(passwordValidator('StrongP@ss1')).toBe(true)
  })

  it('returns error for password without special char', () => {
    expect(passwordValidator('StrongPass1')).toBe(
      'كلمة المرور يجب أن تحتوي على 8 أحرف على الأقل مع أرقام وحروف كبيرة وصغيرة ورمز خاص',
    )
  })
})

describe('confirmPasswordValidator', () => {
  it('returns true when passwords match', () => {
    expect(confirmPasswordValidator('password123', 'password123')).toBe(true)
  })

  it('returns error when passwords do not match', () => {
    expect(confirmPasswordValidator('password123', 'password456')).toBe('كلمتا المرور غير متطابقتين')
  })
})

describe('betweenValidator', () => {
  it('returns true for value within range', () => {
    expect(betweenValidator(5, 1, 10)).toBe(true)
  })

  it('returns true for min value', () => {
    expect(betweenValidator(1, 1, 10)).toBe(true)
  })

  it('returns true for max value', () => {
    expect(betweenValidator(10, 1, 10)).toBe(true)
  })

  it('returns error for value below range', () => {
    expect(betweenValidator(0, 1, 10)).toContain('يجب أن يكون بين')
  })

  it('returns error for value above range', () => {
    expect(betweenValidator(11, 1, 10)).toContain('يجب أن يكون بين')
  })
})

describe('integerValidator', () => {
  it('returns true for empty value', () => {
    expect(integerValidator('')).toBe(true)
  })

  it('returns true for integer string', () => {
    expect(integerValidator('123')).toBe(true)
  })

  it('returns true for negative integer', () => {
    expect(integerValidator('-5')).toBe(true)
  })

  it('returns error for decimal', () => {
    expect(integerValidator('12.5')).toBe('يجب أن يكون عدد صحيح')
  })

  it('returns error for letters', () => {
    expect(integerValidator('abc')).toBe('يجب أن يكون عدد صحيح')
  })
})

describe('regexValidator', () => {
  it('returns true for empty value', () => {
    expect(regexValidator('', /test/)).toBe(true)
  })

  it('returns true for matching value', () => {
    expect(regexValidator('hello123', /^[a-z0-9]+$/i)).toBe(true)
  })

  it('returns error for non-matching value', () => {
    expect(regexValidator('hello!', /^[a-z0-9]+$/i)).toBe('القيمة غير صالحة')
  })

  it('works with string regex', () => {
    expect(regexValidator('123', '^[0-9]+$')).toBe(true)
  })
})

describe('alphaValidator', () => {
  it('returns true for empty value', () => {
    expect(alphaValidator('')).toBe(true)
  })

  it('returns true for alphabetic string', () => {
    expect(alphaValidator('Hello')).toBe(true)
  })

  it('returns error for string with numbers', () => {
    expect(alphaValidator('Hello123')).toBe('يجب أن يحتوي على حروف فقط')
  })
})

describe('urlValidator', () => {
  it('returns true for empty value', () => {
    expect(urlValidator('')).toBe(true)
  })

  it('returns true for valid URL', () => {
    expect(urlValidator('https://example.com')).toBe(true)
  })

  it('returns error for invalid URL', () => {
    expect(urlValidator('not-a-url')).toBe('رابط غير صالح')
  })
})

describe('lengthValidator', () => {
  it('returns true for empty value', () => {
    expect(lengthValidator('', 5)).toBe(true)
  })

  it('returns true for exact length', () => {
    expect(lengthValidator('hello', 5)).toBe(true)
  })

  it('returns error for wrong length', () => {
    expect(lengthValidator('hi', 5)).toContain('يجب أن يكون الطول')
  })
})

describe('alphaDashValidator', () => {
  it('returns true for empty value', () => {
    expect(alphaDashValidator('')).toBe(true)
  })

  it('returns true for alphanumeric with dash/underscore', () => {
    expect(alphaDashValidator('hello-world_123')).toBe(true)
  })

  it('returns error for special characters', () => {
    expect(alphaDashValidator('hello world!')).toBe('يجب أن يحتوي على أحرف وأرقام فقط')
  })
})

describe('validationRules', () => {
  describe('required', () => {
    it('returns error for falsy value', () => {
      expect(validationRules.required('')).toBe('هذا الحقل مطلوب')
    })

    it('returns true for truthy value', () => {
      expect(validationRules.required('hello')).toBe(true)
    })
  })

  describe('email', () => {
    it('returns true for empty value', () => {
      expect(validationRules.email('')).toBe(true)
    })

    it('returns true for valid email', () => {
      expect(validationRules.email('test@example.com')).toBe(true)
    })

    it('returns error for invalid email', () => {
      expect(validationRules.email('invalid')).toBe('البريد الإلكتروني غير صالح')
    })
  })

  describe('phone', () => {
    it('returns true for empty value', () => {
      expect(validationRules.phone('')).toBe(true)
    })

    it('returns true for 10-digit phone', () => {
      expect(validationRules.phone('0512345678')).toBe(true)
    })

    it('returns error for wrong length', () => {
      expect(validationRules.phone('123')).toBe('يجب أن يحتوي على 10 أرقام')
    })
  })

  describe('password', () => {
    it('returns true for empty value', () => {
      expect(validationRules.password('')).toBe(true)
    })

    it('returns true for 8+ chars', () => {
      expect(validationRules.password('12345678')).toBe(true)
    })

    it('returns error for short password', () => {
      expect(validationRules.password('123')).toBe('يجب أن تكون 8 أحرف على الأقل')
    })
  })

  describe('passwordMatch', () => {
    it('returns true for empty value', () => {
      expect(validationRules.passwordMatch('', 'password')).toBe(true)
    })

    it('returns true for matching passwords', () => {
      expect(validationRules.passwordMatch('password', 'password')).toBe(true)
    })

    it('returns error for mismatch', () => {
      expect(validationRules.passwordMatch('diff', 'password')).toBe('كلمتا المرور غير متطابقتين')
    })
  })

  describe('saId', () => {
    it('returns true for empty value', () => {
      expect(validationRules.saId('')).toBe(true)
    })

    it('returns true for 10-digit ID', () => {
      expect(validationRules.saId('1234567890')).toBe(true)
    })

    it('returns error for wrong length', () => {
      expect(validationRules.saId('123')).toBe('يجب أن يكون 10 أرقام')
    })
  })
})
