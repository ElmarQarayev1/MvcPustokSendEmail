$(document).ready(function () {

    $(".book-modal").click(function (e) {
        e.preventDefault();
        let url = this.getAttribute("href");

        fetch(url)
        .then(response => response.text())
        .then(data => {
                $("#quickModal .modal-dialog").html(data)
            })

        $("#quickModal").modal('show');
    })
})

//document.querySelector(".book-modal").addEventListener("click", function (e) {
//    e.preventDefault();
//    alert("salam");
    //$("#quickModal").modal('show');

//})

  
    document.addEventListener('DOMContentLoaded', function () {
        var toastMessage = '@TempData["ToastMessage"]';
        if (toastMessage) {
            var toastElement = document.getElementById('toast');
            var toast = new bootstrap.Toast(toastElement);
            toast.show();
        }
    });


   
   



