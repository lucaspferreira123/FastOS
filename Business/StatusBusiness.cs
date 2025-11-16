using MeuProjeto.Repository;
using TesteMVC.Models;

namespace MeuProjeto.Business
{
    public class StatusBusiness
    {
        private readonly StatusRepository _repository;

        public StatusBusiness(StatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StatusViewModel>> ObterTodosStatus()
        {
            try
            {
                var status = await _repository.ObterTodosStatus();

                return status;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter os status", ex);
            }
        }
    }
}
