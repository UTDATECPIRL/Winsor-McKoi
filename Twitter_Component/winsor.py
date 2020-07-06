import tweepy
import json
from pythonosc import udp_client

class HashtagListener(tweepy.StreamListener):
	def __init__(self):
		super(HashtagListener, self).__init__()
		self.client = udp_client.SimpleUDPClient("127.0.0.1", 5005)
		
	def on_status(self, status):
		if '#feedfish' in status.text:
			self.client.send_message("/feedfish", 1.0)
		else:
			self.client.send_message("/petfish", 1.0)
		
		print(status.text)
		
	def on_error(self, status_code):
		if(status_code == 420):
			return False

tokens = json.load(open("tweepy-tokens.json"))

auth = tweepy.OAuthHandler(tokens['apiToken'], tokens['apiSecret'])
auth.set_access_token(tokens['accessToken'], tokens['accessSecret'])

api = tweepy.API(auth)

hashtagListener = HashtagListener()

hashtagStream = tweepy.Stream(auth = api.auth, listener = hashtagListener)
hashtagStream.filter(track = ['#petfish', '#feedfish'])

