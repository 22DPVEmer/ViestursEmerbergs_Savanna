using System;

namespace Savanna.Services.Exceptions
{
    public class GameServiceException : Exception
    {
        public string UserId { get; }
        public string? GameId { get; }

        public GameServiceException(string message) 
            : base(message)
        {
        }

        public GameServiceException(string message, string userId) 
            : base(message)
        {
            UserId = userId;
        }

        public GameServiceException(string message, string userId, string gameId) 
            : base(message)
        {
            UserId = userId;
            GameId = gameId;
        }

        public GameServiceException(string message, string userId, Exception innerException) 
            : base(message, innerException)
        {
            UserId = userId;
        }

        public GameServiceException(string message, string userId, string gameId, Exception innerException) 
            : base(message, innerException)
        {
            UserId = userId;
            GameId = gameId;
        }
    }
} 