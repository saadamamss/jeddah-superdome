export const saveToStorage = (key, value) => {
  if (!key || !value) return null;

  localStorage.setItem(key, JSON.stringify(value));
};

export const getFromStorage = (key) => {
  if (!key) return null;

  const value = localStorage.getItem(key);

  return value ? JSON.parse(value) : null;
};

export const removeFromStorage = (key) => {
  if (!key) return null;

  localStorage.removeItem(key);
};
