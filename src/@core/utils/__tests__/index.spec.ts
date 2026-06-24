import { describe, expect, it } from 'vitest'
import { isEmpty, isEmptyArray, isNullOrUndefined, isObject, isToday } from '../index'

describe('isEmpty', () => {
  it('returns true for null', () => {
    expect(isEmpty(null)).toBe(true)
  })

  it('returns true for undefined', () => {
    expect(isEmpty(undefined)).toBe(true)
  })

  it('returns true for empty string', () => {
    expect(isEmpty('')).toBe(true)
  })

  it('returns true for empty array', () => {
    expect(isEmpty([])).toBe(true)
  })

  it('returns false for non-empty string', () => {
    expect(isEmpty('hello')).toBe(false)
  })

  it('returns false for non-empty array', () => {
    expect(isEmpty([1, 2, 3])).toBe(false)
  })

  it('returns false for number 0', () => {
    expect(isEmpty(0)).toBe(false)
  })

  it('returns false for boolean false', () => {
    expect(isEmpty(false)).toBe(false)
  })
})

describe('isNullOrUndefined', () => {
  it('returns true for null', () => {
    expect(isNullOrUndefined(null)).toBe(true)
  })

  it('returns true for undefined', () => {
    expect(isNullOrUndefined(undefined)).toBe(true)
  })

  it('returns false for empty string', () => {
    expect(isNullOrUndefined('')).toBe(false)
  })

  it('returns false for 0', () => {
    expect(isNullOrUndefined(0)).toBe(false)
  })

  it('returns false for false', () => {
    expect(isNullOrUndefined(false)).toBe(false)
  })
})

describe('isEmptyArray', () => {
  it('returns true for empty array', () => {
    expect(isEmptyArray([])).toBe(true)
  })

  it('returns false for non-empty array', () => {
    expect(isEmptyArray([1])).toBe(false)
  })

  it('returns false for non-array', () => {
    expect(isEmptyArray('hello')).toBe(false)
  })

  it('returns false for null', () => {
    expect(isEmptyArray(null)).toBe(false)
  })
})

describe('isObject', () => {
  it('returns true for plain object', () => {
    expect(isObject({})).toBe(true)
  })

  it('returns true for object with properties', () => {
    expect(isObject({ key: 'value' })).toBe(true)
  })

  it('returns false for null', () => {
    expect(isObject(null)).toBe(false)
  })

  it('returns false for array', () => {
    expect(isObject([1, 2, 3])).toBe(false)
  })

  it('returns false for string', () => {
    expect(isObject('hello')).toBe(false)
  })

  it('returns false for number', () => {
    expect(isObject(42)).toBe(false)
  })
})

describe('isToday', () => {
  it('returns true for today', () => {
    expect(isToday(new Date())).toBe(true)
  })

  it('returns false for yesterday', () => {
    const yesterday = new Date()
    yesterday.setDate(yesterday.getDate() - 1)
    expect(isToday(yesterday)).toBe(false)
  })

  it('returns false for tomorrow', () => {
    const tomorrow = new Date()
    tomorrow.setDate(tomorrow.getDate() + 1)
    expect(isToday(tomorrow)).toBe(false)
  })
})
