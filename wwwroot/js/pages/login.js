$(document).ready(function () {

});

function ValidarLoginDoUsuarioAoEntrar() {

    console.log('entrou');

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

                if (response === 'Email ou senha inválidos') {
                    return Swal.fire({
                        title: 'Atenção',
                        text: response,
                        icon: 'warning',
                        confirmButtonText: 'Ok'
                    });
                }
                else {
                    return Swal.fire({
                        title: 'Sucesso',
                        text: response,
                        icon: 'success',
                        confirmButtonText: 'Ok'
                    });
                }

                // exemplo de redirecionamento após sucesso:
                // window.location.href = '/Home/Index';
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
