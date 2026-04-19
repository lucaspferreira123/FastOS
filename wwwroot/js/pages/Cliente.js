let clientes = [];
let clientesFiltrados = [];
let paginaAtualCliente = 1;
let linhasPorPaginaCliente = 5;
let idClienteExclusao = 0;

$(document).ready(function () {
    configurarEventosCliente();
    carregarClientes();
});

function configurarEventosCliente() {
    $('#modalCadastroCliente').on('show.bs.modal', function () {
        limparFormularioCadastroCliente();
    });

    $('#modalEditarCliente').on('hidden.bs.modal', function () {
        limparFormularioEdicaoCliente();
    });

    $('#searchInput').on('input', function () {
        paginaAtualCliente = 1;
        aplicarFiltroCliente();
    });

    $('#rowsPerPageSelect').on('change', function () {
        const valor = $(this).val();
        linhasPorPaginaCliente = valor === 'all' ? 'all' : parseInt(valor, 10);
        paginaAtualCliente = 1;
        renderizarTabelaClientes();
    });

    $('#formCadastroCliente').on('submit', async function (event) {
        event.preventDefault();

        const cliente = obterDadosCadastroCliente();
        if (!validarCliente(cliente)) {
            return;
        }

        await $.ajax({
            url: '/Cliente/CadastrarCliente',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(cliente)
        }).done(function () {
            Swal.fire('Sucesso!', 'Cliente cadastrado com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalCadastroCliente')).hide();
            carregarClientes();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel cadastrar o cliente.', 'error');
        });
    });

    $('#formEditarCliente').on('submit', async function (event) {
        event.preventDefault();

        const cliente = obterDadosEdicaoCliente();
        if (!validarCliente(cliente)) {
            return;
        }

        await $.ajax({
            url: '/Cliente/AlterarCliente',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(cliente)
        }).done(function () {
            Swal.fire('Sucesso!', 'Cliente alterado com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalEditarCliente')).hide();
            carregarClientes();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel alterar o cliente.', 'error');
        });
    });

    $('#btnConfirmarExclusao').on('click', async function () {
        if (!idClienteExclusao) {
            return;
        }

        await $.ajax({
            url: `/Cliente/ExcluirCliente/${idClienteExclusao}`,
            type: 'DELETE'
        }).done(function () {
            Swal.fire('Sucesso!', 'Cliente excluido com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalConfirmarExclusao')).hide();
            idClienteExclusao = 0;
            carregarClientes();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel excluir o cliente.', 'error');
        });
    });
}

function carregarClientes() {
    $.ajax({
        url: '/Cliente/ObterClientes',
        type: 'GET'
    }).done(function (response) {
        clientes = response || [];
        aplicarFiltroCliente();
    }).fail(function () {
        Swal.fire('Erro!', 'Nao foi possivel carregar os clientes.', 'error');
    });
}

function aplicarFiltroCliente() {
    const termo = ($('#searchInput').val() || '').trim().toLowerCase();

    clientesFiltrados = clientes.filter(function (cliente) {
        return (cliente.nome || '').toLowerCase().includes(termo) ||
            (cliente.email || '').toLowerCase().includes(termo) ||
            (cliente.telefone || '').toLowerCase().includes(termo);
    });

    renderizarTabelaClientes();
}

function renderizarTabelaClientes() {
    const tbody = $('#tabelaClientes tbody');
    tbody.empty();

    if (clientesFiltrados.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="5" class="text-center text-muted py-4">Nenhum cliente encontrado.</td>
            </tr>
        `);
        $('#pagination').empty();
        return;
    }

    const totalPaginas = linhasPorPaginaCliente === 'all'
        ? 1
        : Math.max(1, Math.ceil(clientesFiltrados.length / linhasPorPaginaCliente));

    if (paginaAtualCliente > totalPaginas) {
        paginaAtualCliente = totalPaginas;
    }

    const inicio = linhasPorPaginaCliente === 'all' ? 0 : (paginaAtualCliente - 1) * linhasPorPaginaCliente;
    const fim = linhasPorPaginaCliente === 'all' ? clientesFiltrados.length : inicio + linhasPorPaginaCliente;
    const pagina = clientesFiltrados.slice(inicio, fim);

    pagina.forEach(function (cliente) {
        tbody.append(`
            <tr>
                <td>${escapeHtml(cliente.nome)}</td>
                <td>${escapeHtml(cliente.email)}</td>
                <td>${escapeHtml(cliente.telefone)}</td>
                <td>${escapeHtml(cliente.endereco)}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-danger me-1" type="button" onclick="abrirModalEdicaoCliente(${cliente.idCliente})">
                        <i class="bi bi-pencil-square"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary" type="button" onclick="abrirModalExclusaoCliente(${cliente.idCliente}, '${escapeJs(cliente.nome)}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });

    renderizarPaginacaoCliente(totalPaginas);
}

function renderizarPaginacaoCliente(totalPaginas) {
    const pagination = $('#pagination');
    pagination.empty();

    if (linhasPorPaginaCliente === 'all') {
        return;
    }

    for (let i = 1; i <= totalPaginas; i++) {
        pagination.append(`
            <li class="page-item ${i === paginaAtualCliente ? 'active' : ''}">
                <button class="page-link" type="button" onclick="irParaPaginaCliente(${i})">${i}</button>
            </li>
        `);
    }
}

function irParaPaginaCliente(pagina) {
    paginaAtualCliente = pagina;
    renderizarTabelaClientes();
}

function abrirModalEdicaoCliente(idCliente) {
    const cliente = clientes.find(function (item) {
        return item.idCliente === idCliente;
    });

    if (!cliente) {
        Swal.fire('Erro!', 'Cliente nao encontrado.', 'error');
        return;
    }

    limparFormularioEdicaoCliente();
    $('#editNomeCliente').val(cliente.nome);
    $('#editEmailCliente').val(cliente.email);
    $('#editTelefoneCliente').val(cliente.telefone);
    $('#editEnderecoCliente').val(cliente.endereco);
    $('#formEditarCliente').data('id-cliente', cliente.idCliente);

    new bootstrap.Modal(document.getElementById('modalEditarCliente')).show();
}

function abrirModalExclusaoCliente(idCliente, nomeCliente) {
    idClienteExclusao = idCliente;
    $('#nomeClienteExclusao').text(nomeCliente);

    new bootstrap.Modal(document.getElementById('modalConfirmarExclusao')).show();
}

function obterDadosCadastroCliente() {
    return {
        nome: $('#nomeCliente').val().trim(),
        email: $('#emailCliente').val().trim(),
        telefone: $('#telefoneCliente').val().trim(),
        endereco: $('#enderecoCliente').val().trim(),
        senha: '',
        ativo: true
    };
}

function obterDadosEdicaoCliente() {
    return {
        idCliente: parseInt($('#formEditarCliente').data('id-cliente'), 10),
        nome: $('#editNomeCliente').val().trim(),
        email: $('#editEmailCliente').val().trim(),
        telefone: $('#editTelefoneCliente').val().trim(),
        endereco: $('#editEnderecoCliente').val().trim(),
        senha: '',
        ativo: true
    };
}

function validarCliente(cliente) {
    if (!cliente.nome) {
        Swal.fire('Atencao!', 'Preencha o nome do cliente.', 'warning');
        return false;
    }

    if (!cliente.email) {
        Swal.fire('Atencao!', 'Preencha o email do cliente.', 'warning');
        return false;
    }

    if (!cliente.telefone) {
        Swal.fire('Atencao!', 'Preencha o telefone do cliente.', 'warning');
        return false;
    }

    return true;
}

function limparFormularioCadastroCliente() {
    $('#formCadastroCliente')[0].reset();
}

function limparFormularioEdicaoCliente() {
    $('#formEditarCliente')[0].reset();
    $('#formEditarCliente').removeData('id-cliente');
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
