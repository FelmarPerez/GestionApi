using GestionApi.Models;

namespace GestionApi.Tests.Models
{
    public class UsuarioTests
    {
        [Fact]
        public void Usuario_DefaultValues_AreSet()
        {
            var user = new Usuario();

            Assert.Equal(string.Empty, user.NombreUsuario);
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.Password);
            Assert.True(user.Activo);
        }
    }
}


