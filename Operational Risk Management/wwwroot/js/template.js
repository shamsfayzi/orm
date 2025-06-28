///=======================================table checkbox
document.addEventListener("DOMContentLoaded", function () {
    let checkAllEl = document.querySelector('#hs-table-checkbox-all')
    if (checkAllEl!=null) {
        checkAllEl.addEventListener("click", function (e) {
            var checkboxes = document.querySelectorAll(".hs-table-checkbox")
            if (e.target.checked) {
                Array.from(checkboxes).filter((item) => item.checked == false).forEach(item => {
                    setTimeout(() => {
                        item.click()
                    }, 0)
                })
            } else {
                Array.from(checkboxes).filter((item) => item.checked).forEach(item => {
                    setTimeout(() => {
                        item.click()
                    }, 0)
                })
            }
        });
        var checkboxes = document.querySelectorAll(".hs-table-checkbox")
        checkboxes.forEach(item => {
            item.addEventListener("click", function (e) {
                if (!e.target.checked) {
                    checkAllEl.checked = false
                }
            })
        })
    }

})

///===========================================clipboard
function copy(text) {
    const textArea = document.createElement("textarea");
    textArea.value = text;
    document.body.appendChild(textArea);
    textArea.select();
    document.execCommand("copy");
    document.body.removeChild(textArea);
    // Alert the copied text
    notify_success("copied!")
}
document.addEventListener("DOMContentLoaded", function () {
    ///==================================================================== tippy tooltip
    tippy('[data-tippy]');
    document.querySelectorAll('[data-clipboard]').forEach(el => {
        el.addEventListener('click', function () {
            copy(this.dataset.clipboard);
        })
    })
})
//============================================= select 
document.addEventListener("DOMContentLoaded", function () {

    function addDataHsSelect() {
        const selectElements = document.querySelectorAll('select.select2');
        const config = {
            "hasSearch": true,
            "searchPlaceholder": "Search...",
            "searchClasses": "block w-full text-sm border-gray-200 rounded-lg focus:border-blue-500 focus:ring-blue-500 before:absolute before:inset-0 before:z-[1] py-2 px-3",
            "searchWrapperClasses": "bg-white p-2 -mx-1 sticky top-0",
            "placeholder": "Select an option...",
            "toggleTag": "<button type=\"button\" aria-expanded=\"false\"><span class=\"text-gray-800\" data-title></span></button>",
            "toggleClasses": "hs-select-disabled:pointer-events-none hs-select-disabled:opacity-50 relative py-3 ps-4 pe-9 flex gap-x-2 text-nowrap w-full cursor-pointer bg-white border border-gray-200 rounded-lg text-start text-sm focus:outline-none focus:ring-2 focus:ring-blue-500",
            "dropdownClasses": "shadow-lg mt-2 max-h-72 pb-1 px-5 space-y-0.5 z-[80] w-full bg-white border border-gray-200 rounded-lg overflow-hidden overflow-y-auto [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-thumb]:rounded-full [&::-webkit-scrollbar-track]:bg-gray-100 [&::-webkit-scrollbar-thumb]:bg-gray-300",
            "optionClasses": "py-2 px-4 w-full text-sm text-gray-800 cursor-pointer hover:bg-gray-100 rounded-lg focus:outline-none focus:bg-gray-100",
            "optionTemplate": "<div><div class=\"flex items-center\"><div class=\"text-gray-800\" data-title></div></div></div>",
            "extraMarkup": "<div class=\"absolute top-1/2 end-3 -translate-y-1/2\"><svg class=\"shrink-0 size-3.5 text-gray-500\" xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"><path d=\"m7 15 5 5 5-5\"/><path d=\"m7 9 5-5 5 5\"/></svg></div>"
        };

        selectElements.forEach(select => {
            select.setAttribute('data-hs-select', JSON.stringify(config));
        });
    }

    // Call the function initially to set the attribute for existing elements
    addDataHsSelect();

    // Optionally, if you need to handle dynamically added elements, you can use MutationObserver
    const observer = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                addDataHsSelect();
            }
        });
    });

    const configObserver = { attributes: true, childList: true, subtree: true };
    observer.observe(document.body, configObserver);
});
////======================================================data ajax unobtrusive
function dataAjaxSuccessRefresh(response) {
    if ($(response).find('#form-successed').val() === 'true') {
        window.location.reload();
    }
}

///=======================================================init vue
const { createApp, ref, reactive } = Vue
/** on ever table with tbody.no-data will have a No Data if no data exist in the table*/
document.addEventListener("DOMContentLoaded", function () {
    var noDataBodies = document.querySelectorAll("tbody.no-data");
    noDataBodies.forEach(function (tbody) {
        if (tbody.querySelectorAll("tr").length === 0) {
            var newRow = document.createElement("tr");
            newRow.classList.add("table-row");

            var newCell = document.createElement("td");
            newCell.classList.add("table-data", "text-center");

            // Get the number of `th` elements in the closest parent table's `thead`
            var thCount = tbody.closest("table").querySelectorAll("thead th").length;
            newCell.setAttribute("colspan", thCount);
            newCell.textContent = "No Data";
            newRow.appendChild(newCell);
            tbody.appendChild(newRow);
        }
    });
});
///=======================================================================loader 
window.addEventListener('load', () => {

    document.getElementById('loader').style.display = 'none';
    document.getElementById('main-content').style.display = 'block';
});

///====================================================================== datatables
window.addEventListener('load', () => {
    const inputs = document.querySelectorAll('.dt-container thead input');
    inputs.forEach((input) => {
        input.addEventListener('keydown', function (evt) {
            if ((evt.metaKey || evt.ctrlKey) && evt.key === 'a') this.select();
        });
    });
});



///================================================================sidebar toggle

window.HSOverlay.autoInit();
const sidebarElementId = "#hs-sidebar-content-push"
const { element } = window.HSOverlay.getInstance(sidebarElementId, true);
const sidebarStatus = localStorage.getItem('sidebar')
if (sidebarStatus != null) {
    if (sidebarStatus === 'open') {
        window.HSOverlay.open(sidebarElementId);
    } else if (sidebarStatus === 'close') {
        window.HSOverlay.close(sidebarElementId);
    }
} else {
    window.HSOverlay.open(sidebarElementId);
    localStorage.setItem('sidebar', 'open')
}
element.on('close', (instance) => {
    localStorage.setItem('sidebar', 'close')
});
element.on('open', (instance) => {
    localStorage.setItem('sidebar', 'open')
});
///================================================================sidebar menue active link
document.addEventListener('DOMContentLoaded', function () {
    const menuItems = document.querySelectorAll('.menu-item');
    const accordionToggles = document.querySelectorAll('.hs-accordion-toggle');

    // Function to set the active class based on the current URL
    function setActiveClass() {
        let currentUrl = window.location.pathname.toLowerCase();
        if (currentUrl.split('/').length < 3) {
            currentUrl = currentUrl+"/index"
        }
        menuItems.forEach(item => {
            // First remove active class from all menu items and accordions
            item.classList.remove('active');
            accordionToggles.forEach(toggle => toggle.classList.remove('active'));

            if (item.href.toLowerCase().includes(currentUrl) && currentUrl !== '/') {
                item.classList.add('active');

                // Check if it's a sub-menu item
                const parentAccordion = item.closest('.hs-accordion');
                if (parentAccordion) {
                    const accordionButton = parentAccordion.querySelector('.hs-accordion-toggle');
                    const accordionContent = parentAccordion.querySelector('.hs-accordion-content');

                    // Expand the parent accordion and set it as active
                    accordionButton.classList.add('active');
                    accordionContent.classList.remove('hidden');
                    accordionButton.setAttribute('aria-expanded', 'true');
                    parentAccordion.classList.add('active'); // Add active class to parent accordion
                }
            }
        });
    }

    // Set the active class on page load based on the current URL
    setActiveClass();

    menuItems.forEach(item => {
        item.addEventListener('click', function (e) {

            // Remove active class from all menu items
            menuItems.forEach(i => i.classList.remove('active'));
            // Remove active class and reset accordion for all toggles
            accordionToggles.forEach(toggle => toggle.classList.remove('active'));

            // Add active class to the clicked menu item
            item.classList.add('active');

            // Check if it's a sub-menu item
            const parentAccordion = item.closest('.hs-accordion');
            if (parentAccordion) {
                const accordionButton = parentAccordion.querySelector('.hs-accordion-toggle');
                const accordionContent = parentAccordion.querySelector('.hs-accordion-content');

                // Expand the parent accordion and set it as active
                accordionButton.classList.add('active');
                accordionContent.classList.remove('hidden');
                accordionButton.setAttribute('aria-expanded', 'true');
                parentAccordion.classList.add('active'); // Add active class to parent accordion
            }
        });
    });
});
