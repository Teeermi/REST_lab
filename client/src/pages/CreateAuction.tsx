import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createAuction } from '../api/auctions';
import { getApiErrorMessage } from '../api/errors';
import { CategoryLabels } from '../types';

const WEEK_IN_MS = 7 * 24 * 60 * 60 * 1000;

const toDatetimeLocalValue = (date: Date) => date.toISOString().slice(0, 16);

const getInitialForm = () => {
  const startDate = new Date();
  const endDate = new Date(startDate.getTime() + WEEK_IN_MS);

  return {
    title: '',
    description: '',
    category: 0,
    startingPrice: '',
    startDate: toDatetimeLocalValue(startDate),
    endDate: toDatetimeLocalValue(endDate),
  };
};

export default function CreateAuction() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [form, setForm] = useState(getInitialForm);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const auction = await createAuction({
        title: form.title,
        description: form.description,
        category: form.category,
        startingPrice: parseFloat(form.startingPrice),
        startDate: new Date(form.startDate).toISOString(),
        endDate: new Date(form.endDate).toISOString(),
      });
      navigate(`/auction/${auction.id}`);
    } catch (err: unknown) {
      setError(getApiErrorMessage(err, 'Błąd tworzenia aukcji'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto px-6 py-8">
      <h1 className="text-2xl font-semibold text-gray-900 mb-6">Nowa aukcja</h1>

      <form onSubmit={handleSubmit} className="space-y-6">
        {error && (
          <div className="bg-red-50 text-red-600 px-4 py-3 rounded-lg text-sm">
            {error}
          </div>
        )}

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Tytuł
          </label>
          <input
            type="text"
            value={form.title}
            onChange={(e) => setForm({ ...form, title: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2"
            required
            maxLength={200}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Opis
          </label>
          <textarea
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2 h-32"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Kategoria
          </label>
          <select
            value={form.category}
            onChange={(e) => setForm({ ...form, category: Number(e.target.value) })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2"
          >
            {Object.entries(CategoryLabels).map(([key, label]) => (
              <option key={key} value={key}>{label}</option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Cena wywoławcza (zł)
          </label>
          <input
            type="number"
            step="0.01"
            min="0.01"
            value={form.startingPrice}
            onChange={(e) => setForm({ ...form, startingPrice: e.target.value })}
            className="w-full border border-gray-300 rounded-lg px-4 py-2"
            required
          />
        </div>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Data rozpoczęcia
            </label>
            <input
              type="datetime-local"
              value={form.startDate}
              onChange={(e) => setForm({ ...form, startDate: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-4 py-2"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Data zakończenia
            </label>
            <input
              type="datetime-local"
              value={form.endDate}
              onChange={(e) => setForm({ ...form, endDate: e.target.value })}
              className="w-full border border-gray-300 rounded-lg px-4 py-2"
              required
            />
          </div>
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 disabled:opacity-50"
        >
          {loading ? 'Tworzenie...' : 'Utwórz aukcję'}
        </button>
      </form>
    </div>
  );
}
