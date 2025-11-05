function warningAlert(message) {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: message,
        icon: 'warning',
        background: '#212529',
        color: '#fff',
    });
}

function infoAlert(message) {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: message,
        icon: 'info',
        background: '#212529',
        color: '#fff',
    });
}

function errorAlert(message) {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: message,
        icon: 'error',
        background: '#212529',
        color: '#fff',
    });
}

function successAlert() {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: "Efetuado com sucesso!",
        text: "\n",
        icon: 'success',
        showConfirmButton: false,
        timer: 700,
        background: '#212529',
        color: '#fff',
    });
}

function successReload() {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: "Efetuado com sucesso!",
        text: "\n",
        icon: 'success',
        showConfirmButton: false,
        background: '#212529',
        color: '#fff',
    }).then((result) => {
        location.reload();
    });

    setTimeout(function () {
        window.location.reload();
    }, 700);
}

function successReloadToURL(url) {
    swal.fire({
        confirmButtonColor: '#0D6EFD',
        title: "Efetuado com sucesso!",
        text: "\n",
        icon: 'success',
        showConfirmButton: false,
        background: '#212529',
        color: '#fff',
    }).then((result) => {
        window.location.href = url;
    });

    setTimeout(function () {
        window.location.href = url;
    }, 700);
}
