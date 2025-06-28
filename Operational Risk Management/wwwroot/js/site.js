$(document).ready(function () {

    $('.select2').select2();

    $(':input').each(function (i, obj) {
        $(obj).attr("autocomplete", "off");
    });
    $('.humanize-date').each(function (i, obj) {
        console.log($(obj).data("date"))
        let hDate = window.humanize(window.toDateTime($(obj).data("date")))
        $(obj)[0].innerHTML = hDate
    });
    window.addEventListener("pageshow", function (event) {
        if (performance.navigation.type == 2) {
            location.reload(true);
        }
    });
    if (document.getElementById("textEditor")) {
        window.editor = new RichTextEditor('#textEditor', {
            editorResizeMode: "none",
            skin: "gray"
        });
    }
});

function toggleCheckboxes(selectAllCheckbox, elClass) {
    const checkboxes = document.querySelectorAll('.' + elClass);
    checkboxes.forEach(checkbox => {
        checkbox.checked = selectAllCheckbox.checked;
    });
}
function toggleCheckbox(id, checked) {
    document.getElementById(id).checked = checked;
}
function formatBytes(bytes, decimals = 2) {
    if (!+bytes) return '0 Bytes'

    const k = 1024
    const dm = decimals < 0 ? 0 : decimals
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']

    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
}
window.accordionOptions = {
    activeClasses: 'bg-white text-gray-900'
};
window.dropdownOptions = {
    placement: 'bottom',
    triggerType: 'click',
    offsetSkidding: 0,
    offsetDistance: 10,
    delay: 300,
    ignoreClickOutsideClass: false
};

window.scrollToBottom = () => {
    var height = document.body.scrollHeight;
    window.scroll(0, height);
}

window.limit = (text) => {
    return string.substring(0, length);
}
window.humanize = (datetime) => {
    return moment(datetime).fromNow()
}
window.toDateTime = (datetime) => {
    return moment(datetime).format("DD MMM YYYY HH:mm")
}
window.scrollToId = (id) => {
    const el = document.getElementById(id)
    el.scrollIntoView(false);
}



