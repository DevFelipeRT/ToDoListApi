import React, { useState, useEffect } from 'react';
import './App.css';

interface ToDo {
  id: number;
  title: string;
  isCompleted: boolean;
  createdAt: string;
  completedAt?: string;
}

function App() {
  const [todos, setTodos] = useState<ToDo[]>([]);
  const [newTodoTitle, setNewTodoTitle] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const API_URL = '/api';

  useEffect(() => {
    fetchTodos();
  }, []);

  const fetchTodos = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${API_URL}/todoitems`);
      const data = await response.json();
      setTodos(data);
      setError('');
    } catch (err) {
      setError('Erro ao carregar tarefas. Verifique se a API está rodando.');
      console.error('Erro ao buscar todos:', err);
    } finally {
      setLoading(false);
    }
  };

  const createTodo = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!newTodoTitle.trim()) {
      alert('Por favor, digite um título para a tarefa');
      return;
    }

    try {
      const response = await fetch(`${API_URL}/todoitems`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ title: newTodoTitle })
      });

      if (response.ok) {
        setNewTodoTitle('');
        fetchTodos();
      }
    } catch (err) {
      console.error('Erro ao criar todo:', err);
      alert('Erro ao criar tarefa');
    }
  };

  const toggleComplete = async (todo: ToDo) => {
    try {
      const endpoint = todo.isCompleted 
        ? `${API_URL}/todoitems/${todo.id}/incomplete`
        : `${API_URL}/todoitems/${todo.id}/complete`;
      
      const response = await fetch(endpoint, { method: 'PATCH' });
      
      if (response.ok) {
        fetchTodos();
      }
    } catch (err) {
      console.error('Erro ao atualizar todo:', err);
      alert('Erro ao atualizar tarefa');
    }
  };

  const deleteTodo = async (id: number) => {
    if (!window.confirm('Tem certeza que deseja excluir esta tarefa?')) {
      return;
    }

    try {
      const response = await fetch(`${API_URL}/todoitems/${id}`, {
        method: 'DELETE'
      });
      
      if (response.ok) {
        fetchTodos();
      }
    } catch (err) {
      console.error('Erro ao deletar todo:', err);
      alert('Erro ao excluir tarefa');
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>📝 Minha Lista de Tarefas</h1>
        <p>Organize suas tarefas de forma simples</p>
      </header>

      <form onSubmit={createTodo} className="todo-form">
        <input
          type="text"
          value={newTodoTitle}
          onChange={(e) => setNewTodoTitle(e.target.value)}
          placeholder="Digite uma nova tarefa..."
          className="todo-input"
          maxLength={150}
        />
        <button type="submit" className="btn btn-add">
          Adicionar
        </button>
      </form>

      {error && <div className="error-message">⚠️ {error}</div>}

      {loading ? (
        <div className="loading">Carregando tarefas...</div>
      ) : (
        <div className="todo-list">
          {todos.length === 0 ? (
            <p className="empty-message">
              Nenhuma tarefa cadastrada. Crie sua primeira tarefa!
            </p>
          ) : (
            todos.map((todo) => (
              <div key={todo.id} className={`todo-item ${todo.isCompleted ? 'completed' : ''}`}>
                <input
                  type="checkbox"
                  checked={todo.isCompleted}
                  onChange={() => toggleComplete(todo)}
                  className="todo-checkbox"
                />
                <div className="todo-content">
                  <span className="todo-title">{todo.title}</span>
                  <small className="todo-date">
                    Criado em: {formatDate(todo.createdAt)}
                  </small>
                </div>
                <button
                  onClick={() => deleteTodo(todo.id)}
                  className="btn btn-delete"
                  title="Excluir tarefa"
                >
                  🗑️
                </button>
              </div>
            ))
          )}
        </div>
      )}

      {todos.length > 0 && (
        <div className="stats">
          <span>Total: {todos.length}</span>
          <span>Concluídas: {todos.filter(t => t.isCompleted).length}</span>
          <span>Pendentes: {todos.filter(t => !t.isCompleted).length}</span>
        </div>
      )}
    </div>
  );
}

export default App;