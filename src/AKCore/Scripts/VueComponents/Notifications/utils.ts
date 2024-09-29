export const allowMovable = (
  draggableElement: HTMLElement,
  removed: () => void,
) => {
  let offsetX: number = 0;
  let initialDiff: number | null = null;

  draggableElement.addEventListener(
    "touchstart",
    (e) => {
      const touch = e.touches[0];

      draggableElement.classList.add("dragged");

      // Calculate offset
      offsetX = touch.clientX - draggableElement.getBoundingClientRect().left;
    },
    { passive: false },
  );

  draggableElement.addEventListener(
    "touchmove",
    (e) => {
      const touch = e.touches[0];

      const diff = touch.clientX - offsetX;
      if (initialDiff === null) {
        initialDiff = diff;
      }

      draggableElement.style.left = `${diff}px`;

      if (Math.abs(diff - initialDiff) > 50) {
        draggableElement.classList.add("removed");
        removed();
      }

      e.preventDefault();
    },
    { passive: false },
  );
};
