let produtos = [];
let produtosFiltrados = [];
let paginaAtualProduto = 1;
let linhasPorPaginaProduto = 5;
let idProdutoExclusao = 0;

$(document).ready(function () {
    configurarEventosProduto();
    carregarProdutos();
});

function configurarEventosProduto() {
    $('#modalCadastroProduto').on('show.bs.modal', function () {
        limparFormularioCadastroProduto();
    });

    $('#modalEditarProduto').on('hidden.bs.modal', function () {
        limparFormularioEdicaoProduto();
    });

    $('#searchInput').on('input', function () {
        paginaAtualProduto = 1;
        aplicarFiltroProduto();
    });

    $('#rowsPerPageSelect').on('change', function () {
        const valor = $(this).val();
        linhasPorPaginaProduto = valor === 'all' ? 'all' : parseInt(valor, 10);
        paginaAtualProduto = 1;
        renderizarTabelaProdutos();
    });

    $('#formCadastroProduto').on('submit', async function (event) {
        event.preventDefault();

        const produto = obterDadosCadastroProduto();
        if (!validarProduto(produto)) {
            return;
        }

        await $.ajax({
            url: '/Produto/CadastrarProduto',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(produto)
        }).done(function () {
            Swal.fire('Sucesso!', 'Produto cadastrado com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalCadastroProduto')).hide();
            carregarProdutos();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel cadastrar o produto.', 'error');
        });
    });

    $('#formEditarProduto').on('submit', async function (event) {
        event.preventDefault();

        const produto = obterDadosEdicaoProduto();
        if (!validarProduto(produto)) {
            return;
        }

        await $.ajax({
            url: '/Produto/AlterarProduto',
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(produto)
        }).done(function () {
            Swal.fire('Sucesso!', 'Produto alterado com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalEditarProduto')).hide();
            carregarProdutos();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel alterar o produto.', 'error');
        });
    });

    $('#btnConfirmarExclusao').on('click', async function () {
        if (!idProdutoExclusao) {
            return;
        }

        await $.ajax({
            url: `/Produto/ExcluirProduto/${idProdutoExclusao}`,
            type: 'DELETE'
        }).done(function () {
            Swal.fire('Sucesso!', 'Produto excluido com sucesso.', 'success');
            bootstrap.Modal.getInstance(document.getElementById('modalConfirmarExclusao')).hide();
            idProdutoExclusao = 0;
            carregarProdutos();
        }).fail(function () {
            Swal.fire('Erro!', 'Nao foi possivel excluir o produto.', 'error');
        });
    });
}

function carregarProdutos() {
    $.ajax({
        url: '/Produto/ObterProdutos',
        type: 'GET'
    }).done(function (response) {
        produtos = response || [];
        aplicarFiltroProduto();
    }).fail(function () {
        Swal.fire('Erro!', 'Nao foi possivel carregar os produtos.', 'error');
    });
}

function aplicarFiltroProduto() {
    const termo = ($('#searchInput').val() || '').trim().toLowerCase();

    produtosFiltrados = produtos.filter(function (produto) {
        return (produto.nomeProduto || '').toLowerCase().includes(termo) ||
            (produto.descricao || '').toLowerCase().includes(termo) ||
            (produto.marca || '').toLowerCase().includes(termo);
    });

    renderizarTabelaProdutos();
}

function renderizarTabelaProdutos() {
    const tbody = $('#tabelaProdutos tbody');
    tbody.empty();

    if (produtosFiltrados.length === 0) {
        tbody.append(`
            <tr>
                <td colspan="6" class="text-center text-muted py-4">Nenhum produto encontrado.</td>
            </tr>
        `);
        $('#pagination').empty();
        return;
    }

    const totalPaginas = linhasPorPaginaProduto === 'all'
        ? 1
        : Math.max(1, Math.ceil(produtosFiltrados.length / linhasPorPaginaProduto));

    if (paginaAtualProduto > totalPaginas) {
        paginaAtualProduto = totalPaginas;
    }

    const inicio = linhasPorPaginaProduto === 'all' ? 0 : (paginaAtualProduto - 1) * linhasPorPaginaProduto;
    const fim = linhasPorPaginaProduto === 'all' ? produtosFiltrados.length : inicio + linhasPorPaginaProduto;
    const pagina = produtosFiltrados.slice(inicio, fim);

    pagina.forEach(function (produto) {
        tbody.append(`
            <tr>
                <td>${escapeHtml(produto.nomeProduto)}</td>
                <td>${escapeHtml(produto.descricao)}</td>
                <td>${escapeHtml(produto.marca)}</td>
                <td>${formatarMoeda(produto.precoUnitario)}</td>
                <td>${escapeHtml(produto.quantidadeTotal)}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-outline-danger me-1" type="button" onclick="abrirModalEdicaoProduto(${produto.idProduto})">
                        <i class="bi bi-pencil-square"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary" type="button" onclick="abrirModalExclusaoProduto(${produto.idProduto}, '${escapeJs(produto.nomeProduto)}')">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `);
    });

    renderizarPaginacaoProduto(totalPaginas);
}

function renderizarPaginacaoProduto(totalPaginas) {
    const pagination = $('#pagination');
    pagination.empty();

    if (linhasPorPaginaProduto === 'all') {
        return;
    }

    for (let i = 1; i <= totalPaginas; i++) {
        pagination.append(`
            <li class="page-item ${i === paginaAtualProduto ? 'active' : ''}">
                <button class="page-link" type="button" onclick="irParaPaginaProduto(${i})">${i}</button>
            </li>
        `);
    }
}

function irParaPaginaProduto(pagina) {
    paginaAtualProduto = pagina;
    renderizarTabelaProdutos();
}

function abrirModalEdicaoProduto(idProduto) {
    const produto = produtos.find(function (item) {
        return item.idProduto === idProduto;
    });

    if (!produto) {
        Swal.fire('Erro!', 'Produto nao encontrado.', 'error');
        return;
    }

    limparFormularioEdicaoProduto();
    $('#formEditarProduto').data('id-produto', produto.idProduto);
    $('#editNomeProduto').val(produto.nomeProduto);
    $('#editDescricaoProduto').val(produto.descricao);
    $('#editPrecoProduto').val(produto.precoUnitario);
    $('#editQuantidadeProduto').val(produto.quantidadeTotal);
    $('#editMarcaProduto').val(produto.marca);

    new bootstrap.Modal(document.getElementById('modalEditarProduto')).show();
}

function abrirModalExclusaoProduto(idProduto, nomeProduto) {
    idProdutoExclusao = idProduto;
    $('#nomeProdutoExclusao').text(nomeProduto);

    new bootstrap.Modal(document.getElementById('modalConfirmarExclusao')).show();
}

function obterDadosCadastroProduto() {
    return {
        nomeProduto: $('#nomeProduto').val().trim(),
        descricao: $('#descricaoProduto').val().trim(),
        precoUnitario: parseFloat($('#precoProduto').val()) || 0,
        quantidadeTotal: parseInt($('#quantidadeProduto').val(), 10) || 0,
        marca: $('#marcaProduto').val().trim()
    };
}

function obterDadosEdicaoProduto() {
    return {
        idProduto: parseInt($('#formEditarProduto').data('id-produto'), 10),
        nomeProduto: $('#editNomeProduto').val().trim(),
        descricao: $('#editDescricaoProduto').val().trim(),
        precoUnitario: parseFloat($('#editPrecoProduto').val()) || 0,
        quantidadeTotal: parseInt($('#editQuantidadeProduto').val(), 10) || 0,
        marca: $('#editMarcaProduto').val().trim()
    };
}

function validarProduto(produto) {
    if (!produto.nomeProduto) {
        Swal.fire('Atencao!', 'Preencha o nome do produto.', 'warning');
        return false;
    }

    if (produto.precoUnitario <= 0) {
        Swal.fire('Atencao!', 'Informe um preco valido.', 'warning');
        return false;
    }

    if (produto.quantidadeTotal < 0) {
        Swal.fire('Atencao!', 'Informe uma quantidade valida.', 'warning');
        return false;
    }

    if (!produto.marca) {
        Swal.fire('Atencao!', 'Preencha a marca do produto.', 'warning');
        return false;
    }

    return true;
}

function limparFormularioCadastroProduto() {
    $('#formCadastroProduto')[0].reset();
}

function limparFormularioEdicaoProduto() {
    $('#formEditarProduto')[0].reset();
    $('#formEditarProduto').removeData('id-produto');
}

function formatarMoeda(valor) {
    return Number(valor || 0).toLocaleString('pt-BR', {
        style: 'currency',
        currency: 'BRL'
    });
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
