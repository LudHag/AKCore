export const slideUpAndDown = (target: HTMLElement, message?: string) => {
  if (message) {
    target.innerHTML = message;
  }

  const originalElement = target.cloneNode(true) as HTMLElement;

  const clonedElement = target.cloneNode(true) as HTMLElement;
  target.removeAttribute("class");
  target.innerHTML = "";
  target.appendChild(clonedElement);

  slideDown(target, clonedElement, 400);
  setTimeout(() => {
    slideUp(target, 400);
    setTimeout(() => {
      target.innerHTML = originalElement.innerHTML;
      Array.from(originalElement.style).forEach((styleName) => {
        target.style.setProperty(
          styleName,
          originalElement.style.getPropertyValue(styleName),
        );
      });

      originalElement.classList.forEach((className) => {
        target.classList.add(className);
      });
      target.appendChild(originalElement);
    }, 400);
  }, 4000);
};

export const slideDown = (
  target: HTMLElement,
  child: HTMLElement,
  duration: number,
) => {
  target.style.overflow = "hidden";
  target.style.height = "0";
  child.style.display = "block";
  target.style.display = "block";
  const height = target.scrollHeight;
  target.style.boxSizing = "border-box";
  target.style.transitionTimingFunction = "linear";
  target.style.transitionProperty = "height";
  target.style.transitionDuration = duration + "ms";
  target.style.height = height + "px";
};

export const slideUp = (target: HTMLElement, duration: number) => {
  target.style.transitionProperty = "height";
  target.style.transitionTimingFunction = "linear";
  target.style.transitionDuration = duration + "ms";
  target.style.boxSizing = "border-box";
  target.style.overflow = "hidden";
  target.style.height = "0";

  window.setTimeout(() => {
    target.style.display = "none";
    target.style.removeProperty("height");
    target.style.removeProperty("overflow");
    target.style.removeProperty("transition-duration");
    target.style.removeProperty("transition-property");
  }, duration);
};
