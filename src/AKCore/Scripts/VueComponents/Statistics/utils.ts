const getStringHash = (str: string): number => {
  let hash = 0;
  if (str.length === 0) return hash;

  for (let i = 0; i < str.length; i++) {
    const char = str.charCodeAt(i);
    hash = (hash << 5) - hash + char;
    hash |= 0; // Convert to 32bit integer
  }

  return hash;
};

const enhanceColorVariance = (
  hash: number,
  varianceFactor: number = 0x777777,
): string => {
  const adjustedHash = (hash ^ varianceFactor) & 0xffffff;
  return adjustedHash.toString(16).padStart(6, "0");
};

const getColorFromHash = (hash: number): string => {
  const hexColor = enhanceColorVariance(hash);
  return `#${hexColor}`;
};

export const getRandomColor = (path: string): string => {
  const hash = getStringHash(path);
  const color = getColorFromHash(hash);
  return color;
};
