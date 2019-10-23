/**
 * @File   : index.tsx
 * @Author : dtysky (dtysky@outlook.com)
 * @Date   : Wed Oct 23 2019
 * @Description: Component.
 */
import {main} from './game';
import './base.scss';

const canvas = document.createElement('canvas');
canvas.className = 'game';
document.getElementById('container').appendChild(canvas);

main(canvas);
