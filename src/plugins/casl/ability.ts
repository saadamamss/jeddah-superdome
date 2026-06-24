import { Ability } from '@casl/ability'
import type { AbilityTuple, MongoQuery } from '@casl/ability'
import type { User } from '@/types'

export type AppAbility = Ability<AbilityTuple, MongoQuery>

export const initialAbility: AppAbility = new Ability([
  {
    action: 'read',
    subject: 'Auth',
  },
]) as AppAbility

export function defineAbilityFromUser(user: User | null): AppAbility {
  if (!user?.permissions?.length) {
    initialAbility.update([{ action: 'read', subject: 'Auth' }])
    return initialAbility
  }

  const rules = user.permissions.map((permission: string) => {
    const [subject, action] = permission.split('.')
    return { action: action || 'manage', subject: subject || 'all' }
  })

  // Always allow reading Auth (Home page visibility for all users)
  rules.push({ action: 'read', subject: 'Auth' })

  initialAbility.update(rules)
  return initialAbility
}

export function getAbility(): AppAbility {
  return initialAbility
}

export default initialAbility
