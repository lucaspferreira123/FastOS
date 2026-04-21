using FastOS.Infrastructure.Repositories;
using FastOS.Domain.Entities;

namespace FastOS.Application.Services
{
    public class StatusBusiness
    {
        private readonly StatusRepository _repository;

        public StatusBusiness(StatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StatusEntity>> ObterTodosStatus()
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

