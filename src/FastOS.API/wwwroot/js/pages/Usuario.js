let usuarios = [];
let usuariosFiltrados = [];
let paginaAtual = 1;
let linhasPorPagina = 5;
let idUsuarioExclusao = 0;

$(document).ready(function () {
    configurarEventos();
    carregarUsuarios();
});

function configurarEventos() {
    $('#modalCadastroUsuario').on('show.bs.modal', function () {
        limparFormularioCadastro();
    });

    $('#modalEditarUsuario').on('hidden.bs.modal', function () {
        limparFormularioEdicao();
    });

    $('#searchInput').on('input', function () {
        paginaAtual = 1;
        aplicarFiltro();
    });

    $('#rowsPerPageSelect').on('change', function () {
        const valor = $(this).val();
        linhasPorPagina = valor === 'all' ? 'all' : parseInt(valor, 10);
        paginaAtual = 1;
        renderizarTabela();
    });

    $('#formCadastroUsuario').on('submit', async function (event) {
        event.preventDefault();

        const usuario = obterDadosCadastro();
        if (!validarUsuario(usuario, $('#confirmarSenhaUsuario').val(), true)) {
            return;
        }

        await $.ajax({
            url: '/Usuario/CadastrarUsuario',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(usuario)
        }).done(function () {
            Swal.fire('Sucesso!', 'Usuario cadastrado com sucesso.', 'success');
            $('#formCadastroUsuario')[0].reset();
            $('#ativoUsuario').prop('checked', true);
            bootstrap.Modal.getInstance(document.getElementById('modalCadastroUsuario')).hide();
            carregarUsuarios();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel cadastrar o usuario.', 'error');
        });
    });

    $('#formEditarUsuario').on('submit', async function (event) {
        event.preventDefault();

        const usuario = obterDadosEdicao();
        if (!validarUsuario(usuario, $('#editConfirmarSenhaUsuario').val(), false)) {
            return;
        }

        await $.ajax({
            url: '/Usuario/AlterarUsuario',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(usuario)
        }).done(function () {
            Swal.fire('Sucesso!', 'Usuario alterado com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalEditarUsuario')).hide();
            carregarUsuarios();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel alterar o usuario.', 'error');
        });
    });

    $('#btnConfirmarExclusao').on('click', async function () {
        if (!idUsuarioExclusao) {
            return;
        }

        await $.ajax({
            url: `/Usuario/ExcluirUsuario/${idUsuarioExclusao}`,
            type: 'DELETE'
        }).done(function () {
            Swal.fire('Sucesso!', 'Usuario excluido com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalConfirmarExclusao')).hide();
            idUsuarioExclusao = 0;
            carregarUsuarios();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel excluir o usuario.', 'error');
        });
    });
}

function carregarUsuarios() {
    $.ajax({
        url: '/Usuario/ObterUsuarios',
        type: 'GET'
    }).done(function (response) {
        usuarios = response || [];
        aplicarFiltro();
    }).fail(function () {
        Swal.fire('Erro!', 'Nao foi possivel carregar os usuarios.', 'error');
    });
}

function aplicarFiltro() {
    const termo = ($('#searchInput').val() || '').trim().toLowerCase();

    usuariosFiltrados = usuarios.filter(function (usuario) {
        const status = usuario.ativo ? 'ativo' : 'inativo';
        return usuario.nome.toLowerCase().includes(termo) ||
            usuario.email.toLowerCase().includes(termo) ||
            (usuario.cargo || '').toLowerCase().includes(termo) ||
            status.includes(termo);
    });

    renderizarTabela();
}

function renderizarTabela() {
    const tbody = $('#tabelaUsuarios tbody');
    tbody.empty();

    if (usuariosFiltrados.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="5" class="text-center text-muted py-4">Nenhum usuario encontrado.</td>
            </tr>
        `);
        $('#pagination').empty();
        return;
    }

    const inicio = linhasPorPagina === 'all' ? 0 : (paginaAtual - 1) * linhasPorPagina;
    const fim = linhasPorPagina === 'all' ? usuariosFiltrados.length : inicio + linhasPorPagina;
    const pagina = usuariosFiltrados.slice(inicio, fim);

    pagina.forEach(function (usuario) {
        const badgeStatus = usuario.ativo
            ? '<span class="badge bg-success-subtle text-success border border-success-subtle">Ativo</span>'
            : '<span class="badge bg-secondary-subtle text-secondary border border-secondary-subtle">Inativo</span>';

        tbody.append(`
            <tr>
                <td>${escapeHtml(usuario.nome)}</td>
                <td>${escapeHtml(usuario.email)}</td>
                <td>${escapeHtml(usuario.cargo || '-')}</td>
                <td>${badgeStatus}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-danger me-1" type="button" onclick="abrirModalEdicao(${usuario.id})">
                        <i class="bi bi-pencil-square"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary" type="button" onclick="abrirModalExclusao(${usuario.id}, '${escapeJs(usuario.nome)}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });

    renderizarPaginacao();
}

function renderizarPaginacao() {
    const pagination = $('#pagination');
    pagination.empty();

    if (linhasPorPagina === 'all') {
        return;
    }

    const totalPaginas = Math.ceil(usuariosFiltrados.length / linhasPorPagina);

    for (let i = 1; i <= totalPaginas; i++) {
        pagination.append(`
            <li class="page-item ${i === paginaAtual ? 'active' : ''}">
                <button class="page-link" type="button" onclick="irParaPagina(${i})">${i}</button>
            </li>
        `);
    }
}

function irParaPagina(pagina) {
    paginaAtual = pagina;
    renderizarTabela();
}

function abrirModalEdicao(idUsuario) {
    const usuario = usuarios.find(function (item) {
        return item.id === idUsuario;
    });

    if (!usuario) {
        Swal.fire('Erro!', 'Usuario nao encontrado.', 'error');
        return;
    }

    limparFormularioEdicao();
    $('#editIdUsuario').val(usuario.id);
    $('#editNomeUsuario').val(usuario.nome);
    $('#editEmailUsuario').val(usuario.email);
    $('#editCargoUsuario').val(usuario.cargo || '');
    $('#editSenhaUsuario').val('');
    $('#editConfirmarSenhaUsuario').val('');
    $('#editAtivoUsuario').prop('checked', usuario.ativo);

    const modal = new bootstrap.Modal(document.getElementById('modalEditarUsuario'));
    modal.show();
}

function abrirModalExclusao(idUsuario, nomeUsuario) {
    idUsuarioExclusao = idUsuario;
    $('#nomeUsuarioExclusao').text(nomeUsuario);

    const modal = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));
    modal.show();
}

function obterDadosCadastro() {
    return {
        nome: $('#nomeUsuario').val().trim(),
        email: $('#emailUsuario').val().trim(),
        senha: $('#senhaUsuario').val(),
        cargo: ($('#cargoUsuario').val() || '').trim() || null,
        ativo: $('#ativoUsuario').is(':checked')
    };
}

function obterDadosEdicao() {
    return {
        id: parseInt($('#editIdUsuario').val(), 10),
        nome: $('#editNomeUsuario').val().trim(),
        email: $('#editEmailUsuario').val().trim(),
        senha: $('#editSenhaUsuario').val(),
        cargo: ($('#editCargoUsuario').val() || '').trim() || null,
        ativo: $('#editAtivoUsuario').is(':checked')
    };
}

function validarUsuario(usuario, confirmarSenha, senhaObrigatoria) {
    if (!usuario.nome) {
        Swal.fire('Atencao!', 'Preencha o nome do usuario.', 'warning');
        return false;
    }

    if (!usuario.email) {
        Swal.fire('Atencao!', 'Preencha o email do usuario.', 'warning');
        return false;
    }

    if (senhaObrigatoria && !usuario.senha) {
        Swal.fire('Atencao!', 'Preencha a senha do usuario.', 'warning');
        return false;
    }

    if (usuario.senha && usuario.senha.length < 4) {
        Swal.fire('Atencao!', 'A senha deve ter pelo menos 4 caracteres.', 'warning');
        return false;
    }

    if (usuario.senha !== confirmarSenha) {
        Swal.fire('Atencao!', 'A confirmacao de senha nao confere.', 'warning');
        return false;
    }

    return true;
}

function escapeHtml(valor) {
    return String(valor ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

function escapeJs(valor) {
    return String(valor ?? '').replace(/\\/g, '\\\\').replace(/'/g, "\\'");
}

function limparFormularioCadastro() {
    $('#formCadastroUsuario')[0].reset();
    $('#ativoUsuario').prop('checked', true);
}

function limparFormularioEdicao() {
    $('#formEditarUsuario')[0].reset();
    $('#editIdUsuario').val('');
    $('#editCargoUsuario').val('');
    $('#editAtivoUsuario').prop('checked', false);
}
