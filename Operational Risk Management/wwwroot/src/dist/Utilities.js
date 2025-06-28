
/// Description: this functionality is used to submit a form via a button outside the form
/// Usage: add data-form-submit to your button with value of the formId
document.addEventListener('DOMContentLoaded', function () {
    const buttons = document.querySelectorAll('[data-form-submit]');

    buttons.forEach(button => {
        button.addEventListener('click', function () {
            const formId = this.getAttribute('data-form-submit');
            const form = document.getElementById(formId);
            if (form) {
                form.submit();
            } else {
                console.error(`Form with ID ${formId} not found`);
            }
        });
    });
});

/// Desctiption: Sending notfication
/// Usage: notify_error(message)
/// Types: error,warning,success,info
function notify_info(msg) {
    const template = ` <div class="max-w-xs bg-blue-100 rounded-xl shadow-lg" role="alert" tabindex="-1" aria-labelledby="hs-toast-normal-example-label">
    <div class="flex p-4 items-center">
      <div class="shrink-0">
        <svg class="shrink-0 size-4 text-blue-500 mt-0.5" xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
          <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm.93-9.412-1 4.705c-.07.34.029.533.304.533.194 0 .487-.07.686-.246l-.088.416c-.287.346-.92.598-1.465.598-.703 0-1.002-.422-.808-1.319l.738-3.468c.064-.293.006-.399-.287-.47l-.451-.081.082-.381 2.29-.287zM8 5.5a1 1 0 1 1 0-2 1 1 0 0 1 0 2z"></path>
        </svg>
      </div>
      <div class="pl-3">
        <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          `+msg+`
        </p>
      </div>
    </div>
  </div>`
    const msgAlert = alertify.message()
    msgAlert.setContent(template)
}

function notify_success(msg) {
    const template = ` <div class="max-w-xs bg-green-100 rounded-xl shadow-lg" role="alert" tabindex="-1" aria-labelledby="hs-toast-normal-example-label">
    <div class="flex p-4 items-center">
      <div class="shrink-0">
        <svg class="shrink-0 size-4 text-green-500 mt-0.5" xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
          <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"></path>
        </svg>
      </div>
      <div class="pl-3">
        <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          `+ msg +`
        </p>
        </p>
      </div>
    </div>
  </div>`
    const msgAlert = alertify.success()
    msgAlert.setContent(template)
}

function notify_warning(msg) {
    const template = ` <div class="max-w-xs bg-yellow-100 rounded-xl shadow-lg" role="alert" tabindex="-1" aria-labelledby="hs-toast-normal-example-label">
    <div class="flex p-4 items-center">
      <div class="shrink-0">
      <svg class="shrink-0 size-4 text-yellow-500 mt-0.5" xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
          <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8 4a.905.905 0 0 0-.9.995l.35 3.507a.552.552 0 0 0 1.1 0l.35-3.507A.905.905 0 0 0 8 4zm.002 6a1 1 0 1 0 0 2 1 1 0 0 0 0-2z"></path>
        </svg>
      </div>
      <div class="pl-3">
        <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          `+ msg + `
        </p>
        </p>
      </div>
    </div>
  </div>`
    const msgAlert = alertify.warning()
    msgAlert.setContent(template)
}

function notify_error(msg) {
    const template = ` <div class="max-w-xs bg-red-100 rounded-xl shadow-lg" role="alert" tabindex="-1" aria-labelledby="hs-toast-normal-example-label">
    <div class="flex p-4 items-center">
      <div class="shrink-0">
        <svg class="shrink-0 size-4 text-red-500 mt-0.5" xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
          <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM5.354 4.646a.5.5 0 1 0-.708.708L7.293 8l-2.647 2.646a.5.5 0 0 0 .708.708L8 8.707l2.646 2.647a.5.5 0 0 0 .708-.708L8.707 8l2.647-2.646a.5.5 0 0 0-.708-.708L8 7.293 5.354 4.646z"></path>
        </svg>
      </div>
      <div class="pl-3">
        <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          <p id="hs-toast-normal-example-label" class="text-sm text-gray-700">
          `+ msg + `
        </p>
        </p>
      </div>
    </div>
  </div>`
    const msgAlert = alertify.error()
    msgAlert.setContent(template)
}

/// Description: navigate back
/// Usage: just assign btn-back to any button
$('.btn-back').on("click", function () {
    history.back()
})

/// Description: request for delete operation
/// Usage: assign the btn-delete on any button and also set the data-delete-url and data-delete-elementId
$('.btn-delete').on("click", function (e) {
    const url = $(this).attr('data-delete-url')
    const deleteElmId = $(this).attr('data-delete-elementId')
    deleteConfirmation(url, deleteElmId)
})

/// Description:show a popup for delete confirmation, Note: delete method should return true or false indicating the operation result
const deleteConfirmation=(url,id)=>{
    alertify.confirm("Delete Operation", "Are you sure to delete this record?",
        function () {
            axios.post(url)
                .then(function (response) {
                    if (response.data) {
                        $('#'+id).hide(300);
                        notify_success("Delete operation done successfully.")
                    } else {
                        notify_error("Record not deleted.")
                    }
                }).catch(function (error) {
                    notify_error("En error occured during the delete opertaion.")
                    console.log(error);
                });
        },
        function () {
            
        });
}
