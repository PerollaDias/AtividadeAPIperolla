using Atividade_João___Pérolla;



namespace Atividade_João___Pérolla.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class FornecedorController : Controller
    {
        private const string Arquivotxt = "fornecedores.txt";

        private bool ValidarCNPJ(string cnpj) //Exemplo adaptado funcionando!
        {
            cnpj = Regex.Replace(cnpj, @"\D", "");
            return cnpj.Length == 14;
        }

        private List<Fornecedor> LerFornecedoresDoArquivo() //Exemplo adaptado funcionando ok!
        {
            var fornecedores = new List<Fornecedor>();

            if (!System.IO.File.Exists(Arquivotxt))
            {
                return fornecedores;
            }

            var linhas = System.IO.File.ReadAllLines(Arquivotxt);
            foreach (var linha in linhas)
            {
                var dados = linha.Split('|');
                fornecedores.Add(new Fornecedor
                {
                    NomeFantasia = dados[0],
                    RazaoSocial = dados[1],
                    CNPJ = dados[2],
                    Endereco = dados[3],
                    Cidade = dados[4],
                    Estado = dados[5],
                    Telefone = dados[6],
                    Email = dados[7],
                    Responsavel = dados[8]
                });
            }

            return fornecedores;
        }

        private void GravandoFornecedoresNoArquivo(List<Fornecedor> fornecedores) //Tudo funcionando!
        {
            var linhas = fornecedores.Select(f => $"{f.NomeFantasia}|" +
            $"{f.RazaoSocial}|" +
            $"{f.CNPJ}|" +
            $"{f.Endereco}|" +
            $"{f.Cidade}|" +
            $"{f.Estado}|" +
            $"{f.Telefone}|" +
            $"{f.Email}|" +
            $"{f.Responsavel}");
            System.IO.File.WriteAllLines(Arquivotxt, linhas);
        }

        [HttpGet]
        public IActionResult Get() //Leitura funcionando!
        {
            var fornecedores = LerFornecedoresDoArquivo();
            return Ok(fornecedores);
        }

        [HttpGet("{cnpj}")]
        public IActionResult GetByCNPJ(string cnpj) //Exemplo adaptado funcionando!
        {
            if (!ValidarCNPJ(cnpj))
            {
                return BadRequest("CNPJ inválido.");
            }

            var fornecedores = LerFornecedoresDoArquivo();
            var fornecedor = fornecedores.FirstOrDefault(f => f.CNPJ == cnpj);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return Ok(fornecedor);
        }

        [HttpPost]
        public IActionResult Post([FromBody] FornecedorDTO dto) //Validação adaptada funcionando!
        {
            if (dto == null)
            {
                return BadRequest("Dados inválidos. Verifique novamente!");
            }

            var cnpj = dto.CNPJ;
            if (!ValidarCNPJ(cnpj))
            {
                return BadRequest("Desculpe, CNPJ inválido.");
            }

            var fornecedores = LerFornecedoresDoArquivo();
            if (fornecedores.Any(f => f.CNPJ == cnpj))
            {
                return Conflict("Fornecedor já cadastrado.");
            }

            var fornecedor = new Fornecedor
            {
                NomeFantasia = dto.NomeFantasia,
                RazaoSocial = dto.RazaoSocial,
                CNPJ = cnpj,
                Endereco = dto.Endereco,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                Telefone = dto.Telefone,
                Email = dto.Email,
                Responsavel = dto.Responsavel
            };

            fornecedores.Add(fornecedor);
            GravandoFornecedoresNoArquivo(fornecedores);

            return CreatedAtAction(nameof(GetByCNPJ), new { cnpj = fornecedor.CNPJ }, fornecedor);
        }

        [HttpPut("{cnpj}")]
        public IActionResult Put(string cnpj, [FromBody] FornecedorDTO dto) //Validação adaptada para ler funcionando!
        {
            if (!ValidarCNPJ(cnpj))
            {
                return BadRequest("CNPJ inválido.");
            }

            var fornecedores = LerFornecedoresDoArquivo();
            var fornecedor = fornecedores.FirstOrDefault(f => f.CNPJ == cnpj);

            if (fornecedor == null)
            {
                return NotFound();
            }

            fornecedor.NomeFantasia = dto.NomeFantasia ?? fornecedor.NomeFantasia;
            fornecedor.RazaoSocial = dto.RazaoSocial ?? fornecedor.RazaoSocial;
            fornecedor.Endereco = dto.Endereco ?? fornecedor.Endereco;
            fornecedor.Cidade = dto.Cidade ?? fornecedor.Cidade;
            fornecedor.Estado = dto.Estado ?? fornecedor.Estado;
            fornecedor.Telefone = dto.Telefone ?? fornecedor.Telefone;
            fornecedor.Email = dto.Email ?? fornecedor.Email;
            fornecedor.Responsavel = dto.Responsavel ?? fornecedor.Responsavel;

            GravandoFornecedoresNoArquivo(fornecedores);

            return Ok(fornecedor);
        }

        [HttpDelete("{cnpj}")]
        public IActionResult Delete(string cnpj) //Validação adaptada para deletar funcionando!
        {
            if (!ValidarCNPJ(cnpj))
            {
                return BadRequest("CNPJ inválido. Tente novamente!");
            }

            var fornecedores = LerFornecedoresDoArquivo();
            var fornecedor = fornecedores.FirstOrDefault(f => f.CNPJ == cnpj);

            if (fornecedor == null)
            {
                return NotFound();
            }

            fornecedores.Remove(fornecedor);
            GravandoFornecedoresNoArquivo(fornecedores);

            return Ok(fornecedor);
        }
    }
}