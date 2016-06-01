import test from 'ava';
import Layout from './layout';

test.before('setup', () => {
    this.view = new Layout();
    this.view.render();
});

test('It exports a "pagination" region', t => {
    this.view["pagination"]._ensureElement();
    t.is(this.view["pagination"].$el.length, 1, 'message');
});
