function toggleCopyButtons(id) {
  if (!document.querySelector(`${id} [role="tablist"]`)) return false;

  const tabs = HSTabs.getInstance(`${id} [role="tablist"]`);

  tabs.on('change', ({ _, current }) => {
    const copyButtons = document.querySelectorAll(`${id} [data-hs-associated-tab]`);

    copyButtons.forEach((elI) => {
      const associatedTab = JSON.parse(elI.getAttribute('data-hs-associated-tab'));

      if (associatedTab.includes(current)) {
        elI.classList.remove('hidden');
        elI.classList.add('inline-flex');
      } else {
        elI.classList.remove('inline-flex');
        elI.classList.add('hidden');
      }
    });
  });
}

function toggleCopyButtonsAlt(parent) {
  if (!parent && !parent.querySelector('[role="tablist"]')) return false;

  const tabs = HSTabs.getInstance(parent.querySelector('[role="tablist"]'));

  tabs.on('change', ({ _, current }) => {
    const copyButtons = parent.querySelectorAll('[data-hs-associated-tab]');

    copyButtons.forEach((elI) => {
      const associatedTab = elI.getAttribute('data-hs-associated-tab');

      if (associatedTab.includes(current)) {
        elI.classList.remove('hidden');
        elI.classList.add('inline-flex');
      } else {
        elI.classList.remove('inline-flex');
        elI.classList.add('hidden');
      }
    });
  });
}

window.addEventListener('load', () => {
  (function () {
    const copyCodeTabs = document.querySelectorAll('.hs-copy-code');

    copyCodeTabs.forEach((parent) => toggleCopyButtonsAlt(parent));
  })();
});