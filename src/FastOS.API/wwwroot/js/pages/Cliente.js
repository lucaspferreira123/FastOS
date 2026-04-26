const TIPO_CLIENTE = {
    PESSOA_FISICA: 1,
    PESSOA_JURIDICA: 2
};

let clientes = [];
let idClienteExclusao = 0;

$(document).ready(function () {
    configurarEventosCliente();
    carregarClientes();
});

function configurarEventosCliente() {
    $('#modalCadastroCliente').on('show.bs.modal', function () {
        limparFormularioCadastroCliente();
        alternarCamposDocumentoCadastro();
    });

    $('#modalEditarCliente').on('hidden.bs.modal', function () {
        limparFormularioEdicaoCliente();
    });

    $('#searchInput').on('input', function () {
        renderizarTabelasClientes();
    });

    $('#tipoClienteCadastro').on('change', function () { alternarCamposDocumentoCadastro(true); });
    $('#editTipoCliente').on('change', function () { alternarCamposDocumentoEdicao(true); });

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
        }).fail(function (xhr) {
            Swal.fire('Erro!', obterMensagemErro(xhr, 'Nao foi possivel cadastrar o cliente.'), 'error');
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
        }).fail(function (xhr) {
            Swal.fire('Erro!', obterMensagemErro(xhr, 'Nao foi possivel alterar o cliente.'), 'error');
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
        }).fail(function (xhr) {
            Swal.fire('Erro!', obterMensagemErro(xhr, 'Nao foi possivel excluir o cliente.'), 'error');
        });
    });
}

function carregarClientes() {
    $.ajax({
        url: '/Cliente/ObterClientes',
        type: 'GET'
    }).done(function (response) {
        clientes = response || [];
        renderizarTabelasClientes();
    }).fail(function () {
        Swal.fire('Erro!', 'Nao foi possivel carregar os clientes.', 'error');
    });
}

function renderizarTabelasClientes() {
    const termo = ($('#searchInput').val() || '').trim().toLowerCase();
    const clientesFiltrados = clientes.filter(function (cliente) {
        const documento = obterDocumentoCliente(cliente).toLowerCase();
        return (cliente.nome || '').toLowerCase().includes(termo) || documento.includes(termo);
    });

    const clientesAtivos = clientesFiltrados.filter(function (cliente) {
        return !!cliente.ativo;
    });

    const clientesInativos = clientesFiltrados.filter(function (cliente) {
        return !cliente.ativo;
    });

    renderizarTabela('#tabelaClientesAtivos tbody', clientesAtivos);
    renderizarTabela('#tabelaClientesInativos tbody', clientesInativos);
}

function renderizarTabela(selector, lista) {
    const tbody = $(selector);
    tbody.empty();

    if (lista.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="2" class="text-center text-muted py-4">Nenhum cliente encontrado.</td>
            </tr>
        `);
        return;
    }

    lista.forEach(function (cliente) {
        tbody.append(`
            <tr>
                <td>${escapeHtml(cliente.nome)} - ${escapeHtml(formatarDocumento(obterDocumentoCliente(cliente), cliente.tipoCliente))}</td>
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
    $('#editTipoCliente').val(cliente.tipoCliente);
    $('#editEmailCliente').val(cliente.email);
    $('#editTelefoneCliente').val(cliente.telefone);
    $('#editEnderecoCliente').val(cliente.endereco);
    $('#editAtivoCliente').val(String(cliente.ativo));
    $('#editCpfCliente').val(formatarDocumento(cliente.cpf, TIPO_CLIENTE.PESSOA_FISICA));
    $('#editCnpjCliente').val(formatarDocumento(cliente.cnpj, TIPO_CLIENTE.PESSOA_JURIDICA));
    $('#formEditarCliente').data('id-cliente', cliente.idCliente);
    alternarCamposDocumentoEdicao(false);

    new bootstrap.Modal(document.getElementById('modalEditarCliente')).show();
}

function abrirModalExclusaoCliente(idCliente, nomeCliente) {
    idClienteExclusao = idCliente;
    $('#nomeClienteExclusao').text(nomeCliente);
    new bootstrap.Modal(document.getElementById('modalConfirmarExclusao')).show();
}

function obterDadosCadastroCliente() {
    const tipoCliente = parseInt($('#tipoClienteCadastro').val(), 10);
    return {
        nome: $('#nomeCliente').val().trim(),
        tipoCliente: tipoCliente,
        email: $('#emailCliente').val().trim(),
        telefone: $('#telefoneCliente').val().trim(),
        endereco: $('#enderecoCliente').val().trim(),
        ativo: $('#ativoClienteCadastro').val() === 'true',
        cpf: tipoCliente === TIPO_CLIENTE.PESSOA_FISICA ? $('#cpfCliente').val().trim() : null,
        cnpj: tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA ? $('#cnpjCliente').val().trim() : null
    };
}

function obterDadosEdicaoCliente() {
    const tipoCliente = parseInt($('#editTipoCliente').val(), 10);
    return {
        idCliente: parseInt($('#formEditarCliente').data('id-cliente'), 10),
        nome: $('#editNomeCliente').val().trim(),
        tipoCliente: tipoCliente,
        email: $('#editEmailCliente').val().trim(),
        telefone: $('#editTelefoneCliente').val().trim(),
        endereco: $('#editEnderecoCliente').val().trim(),
        ativo: $('#editAtivoCliente').val() === 'true',
        cpf: tipoCliente === TIPO_CLIENTE.PESSOA_FISICA ? $('#editCpfCliente').val().trim() : null,
        cnpj: tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA ? $('#editCnpjCliente').val().trim() : null
    };
}

function validarCliente(cliente) {
    if (!cliente.nome) {
        Swal.fire('Atencao!', 'Preencha a razao social do cliente.', 'warning');
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

    if (cliente.tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA) {
        const cnpj = normalizarDocumento(cliente.cnpj);
        if (!cnpj) {
            Swal.fire('Atencao!', 'Informe o CNPJ do cliente.', 'warning');
            return false;
        }

        if (cnpj.length !== 14) {
            Swal.fire('Atencao!', 'O CNPJ deve conter 14 numeros.', 'warning');
            return false;
        }
    } else {
        const cpf = normalizarDocumento(cliente.cpf);
        if (!cpf) {
            Swal.fire('Atencao!', 'Informe o CPF do cliente.', 'warning');
            return false;
        }

        if (cpf.length !== 11) {
            Swal.fire('Atencao!', 'O CPF deve conter 11 numeros.', 'warning');
            return false;
        }
    }

    return true;
}

function alternarCamposDocumentoCadastro(limparCampoInativo) {
    const tipoCliente = parseInt($('#tipoClienteCadastro').val(), 10);
    const pessoaJuridica = tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA;

    $('.campo-documento-cadastro[data-tipo="cpf"]').toggleClass('d-none', pessoaJuridica);
    $('.campo-documento-cadastro[data-tipo="cnpj"]').toggleClass('d-none', !pessoaJuridica);

    if (limparCampoInativo && pessoaJuridica) {
        $('#cpfCliente').val('');
    } else if (limparCampoInativo) {
        $('#cnpjCliente').val('');
    }
}

function alternarCamposDocumentoEdicao(limparCampoInativo) {
    const tipoCliente = parseInt($('#editTipoCliente').val(), 10);
    const pessoaJuridica = tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA;

    $('.campo-documento-edicao[data-tipo="cpf"]').toggleClass('d-none', pessoaJuridica);
    $('.campo-documento-edicao[data-tipo="cnpj"]').toggleClass('d-none', !pessoaJuridica);

    if (limparCampoInativo && pessoaJuridica) {
        $('#editCpfCliente').val('');
    } else if (limparCampoInativo) {
        $('#editCnpjCliente').val('');
    }
}

function limparFormularioCadastroCliente() {
    $('#formCadastroCliente')[0].reset();
    $('#tipoClienteCadastro').val(String(TIPO_CLIENTE.PESSOA_FISICA));
    $('#ativoClienteCadastro').val('true');
}

function limparFormularioEdicaoCliente() {
    $('#formEditarCliente')[0].reset();
    $('#formEditarCliente').removeData('id-cliente');
    $('#editTipoCliente').val(String(TIPO_CLIENTE.PESSOA_FISICA));
    $('#editAtivoCliente').val('true');
    alternarCamposDocumentoEdicao(false);
}

function obterDocumentoCliente(cliente) {
    return cliente.tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA ? (cliente.cnpj || '') : (cliente.cpf || '');
}

function normalizarDocumento(valor) {
    return String(valor || '').replace(/\D/g, '');
}

function formatarDocumento(valor, tipoCliente) {
    const documento = normalizarDocumento(valor);

    if (!documento) {
        return '';
    }

    if (tipoCliente === TIPO_CLIENTE.PESSOA_JURIDICA && documento.length === 14) {
        return documento.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
    }

    if (tipoCliente === TIPO_CLIENTE.PESSOA_FISICA && documento.length === 11) {
        return documento.replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, '$1.$2.$3-$4');
    }

    return documento;
}

function obterMensagemErro(xhr, mensagemPadrao) {
    return xhr && xhr.responseText ? xhr.responseText : mensagemPadrao;
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
