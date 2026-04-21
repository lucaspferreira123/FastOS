$(document).ready(function () {

});

function ValidarLoginDoUsuarioAoEntrar() {

    var email = $('#txtEmailLogin').val();
    var senha = $('#txtSenhaLogin').val();

    var validado = ValidarCamposLogin(email, senha);

    if (validado == true) {

        $.ajax({
            url: '/Login/VerificarLoginUsuarioAoLogar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                Email: email,
                Password: senha
            }),
            success: function (response) {

                var redirect = response && response.redirectUrl ? response.redirectUrl : '/Home/Index';
                return window.location.href = redirect;

            },
            error: function (xhr) {
                if (xhr.status === 400 || xhr.status === 401) {
                    Swal.fire({
                        title: 'Erro',
                        text: xhr.responseText,
                        icon: 'error'
                    });
                } else {
                    Swal.fire({
                        title: 'Erro',
                        text: 'Login ou senha invalidos.',
                        icon: 'error'
                    });
                }
            }
        });
    }
}

function ValidarCamposLogin(email, senha) {

   
    if (email === '' || email === null) {
        Swal.fire({
            title: 'Atenção',
            text: 'Preencha o campo de email!',
            icon: 'warning'
        });
        return false;
    }

    if (senha === '' || senha === null) {
        Swal.fire({
            title: 'Atenção',
            text: 'Preencha o campo de senha!',
            icon: 'warning'
        });        return false;
    }

    return true;

}
